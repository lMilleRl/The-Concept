using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TypeRunner : ITypeRunner, IDisposable
    {
        private sealed class TypingSession
        {
            public bool IsCancelled { get; private set; }

            public void Cancel() => IsCancelled = true;
        }

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IDebugWriter _debugWriter;

        private ITextBoxUI _currentTextBoxUI;
        private TMP_TextInfo _currentTextInfo;
        private Coroutine _turnPagesCoroutine;
        private Coroutine _typePageCoroutine;
        private Coroutine _easeSpeedCoroutine;
        private TypingSession _currentSession;

        private float _perCharPause;
        private float _currentPause;
        private bool _canTurnPage;
        private int _currentVisibleChars;
        private int _startPosition;

        public event Action OnPageStarted;
        public event Action OnPageFinished;
        public event Action OnTextFinished;
        public event Action<int> OnCharRevealed;
        public event Action<char> OnCharPrinted;

        public TypeRunner(ICoroutineRunner coroutineRunner, IDebugWriter debugWriter)
        {
            _coroutineRunner = coroutineRunner;
            _debugWriter = debugWriter;

            OnPageFinished += HandlePageFinished;
        }

        public void Dispose()
        {
            Stop();
            OnPageFinished -= HandlePageFinished;
        }

        private void HandlePageFinished() => _isPageFinished = true;

        public void Run(ITextBoxUI ui)
        {
            Stop();
            _currentTextBoxUI = ui;
            _currentTextInfo = _currentTextBoxUI.GetTextInfo();

            if (_currentTextInfo == null)
            {
                _debugWriter.LogWarning($"[{nameof(TypeRunner)}] textInfo is null on Run.");
                return;
            }

            int maxChars = _currentTextInfo.characterCount;
            if (_startPosition > maxChars)
            {
                _debugWriter.LogWarning(
                    $"[{nameof(TypeRunner)}] Start position {_startPosition} exceeds character count {maxChars}. Clamping.");
                _startPosition = maxChars;
            }

            _currentVisibleChars = _startPosition;

            _currentSession = new TypingSession();
            _turnPagesCoroutine = _coroutineRunner.StartCoroutine(TurnPages(_currentSession));
        }

        public void SetPosition(ITextBoxUI ui, int charIndex)
        {
            _startPosition = Mathf.Max(0, charIndex);

            if (ui == null)
                return;

            _currentTextBoxUI = ui;
            _currentTextInfo = ui.GetTextInfo();

            if (_currentTextInfo == null)
            {
                _debugWriter.LogWarning($"[{nameof(TypeRunner)}] textInfo is null on SetPosition.");
                return;
            }

            int maxChars = _currentTextInfo.characterCount;
            if (_startPosition > maxChars)
            {
                _debugWriter.LogWarning(
                    $"[{nameof(TypeRunner)}] Start position {_startPosition} exceeds character count {maxChars}. Clamping.");
                _startPosition = maxChars;
            }
        }

        public void Stop()
        {
            _currentSession?.Cancel();
            _currentSession = null;

            if (_typePageCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_typePageCoroutine);
                _typePageCoroutine = null;
            }

            if (_turnPagesCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_turnPagesCoroutine);
                _turnPagesCoroutine = null;
            }

            if (_easeSpeedCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_easeSpeedCoroutine);
                _easeSpeedCoroutine = null;
            }
        }

        public void SetSpeed(float charsPerSecond)
        {
            _perCharPause = 1f / charsPerSecond;
        }

        public void SetSpeedEase(float targetCharsPerSecond, int startCharIndex, int charLength, EaseType ease)
        {
            if (charLength <= 0)
            {
                SetSpeed(targetCharsPerSecond);
                return;
            }

            if (_easeSpeedCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_easeSpeedCoroutine);
            }

            _easeSpeedCoroutine = _coroutineRunner.StartCoroutine(
                EaseSpeedByChars(targetCharsPerSecond, startCharIndex, startCharIndex + charLength, ease));
        }

        private IEnumerator EaseSpeedByChars(float targetCharsPerSecond, int startCharIndex, int endCharIndex,
            EaseType ease)
        {
            float targetPause = 1f / targetCharsPerSecond;
            float startPause = _perCharPause;
            int length = endCharIndex - startCharIndex;

            while (_currentVisibleChars < endCharIndex)
            {
                float t = Mathf.Clamp01((float)(_currentVisibleChars - startCharIndex) / length);
                _perCharPause = Mathf.Lerp(startPause, targetPause, ApplyEase(t, ease));
                yield return null;
            }

            _perCharPause = targetPause;
            _easeSpeedCoroutine = null;
        }

        private static float ApplyEase(float t, EaseType ease)
        {
            return ease switch
            {
                EaseType.None => 1f,
                EaseType.Linear => t,
                EaseType.EaseInQuad => t * t,
                _ => t
            };
        }

        public void SetPause(float seconds)
        {
            _currentPause = seconds;
        }


        private bool _isPageFinished;

        public void TurnToNextPage()
        {
            if (_isPageFinished)
            {
                _canTurnPage = true;
                _isPageFinished = false;
            }
        }

        public int GetCurrentVisibleChars()
        {
            return _currentVisibleChars;
        }

        public float GetProgress(int charIndex, int startCharIndex, int charLength, EaseType ease)
        {
            if (charLength <= 0)
                return 1f;

            float t = Mathf.Clamp01(
                (float)(charIndex - startCharIndex  + 1) / charLength);

            return ApplyEase(t, ease);
        }

        private IEnumerator TurnPages(TypingSession session)
        {
            yield return new WaitUntil(() => session.IsCancelled || _currentTextBoxUI.IsTextInitialized);
            if (session.IsCancelled)
                yield break;

            _currentTextInfo = _currentTextBoxUI.GetTextInfo();
            if (_currentTextInfo == null)
            {
                _debugWriter.LogWarning($"[{nameof(TypeRunner)}] textInfo is null in TurnPages after WaitUntil.");
                yield break;
            }

            int pageCount = _currentTextBoxUI.GetPageCount();
            int startPage = _currentTextBoxUI.GetPageIndexForChar(_startPosition);

            for (int page = startPage; page < pageCount; page++)
            {
                if (session.IsCancelled)
                    yield break;

                if (page != startPage)
                {
                    int firstChar = _currentTextInfo.pageInfo[page].firstCharacterIndex;
                    _currentTextBoxUI.ContentText.maxVisibleCharacters = firstChar;
                    _currentTextBoxUI.ContentText.pageToDisplay = page + 1;
                }

                OnPageStarted?.Invoke();
                if (session.IsCancelled)
                    yield break;

                _typePageCoroutine = _coroutineRunner.StartCoroutine(TypePage(page, page == startPage, session));
                yield return _typePageCoroutine;
                if (session.IsCancelled)
                    yield break;

                OnPageFinished?.Invoke();
                if (session.IsCancelled)
                    yield break;

                yield return new WaitUntil(() => session.IsCancelled || _canTurnPage);
                if (session.IsCancelled)
                    yield break;

                _canTurnPage = false;
            }

            OnTextFinished?.Invoke();
        }

        private IEnumerator TypePage(int pageIndex, bool isStartPage, TypingSession session)
        {
            TMP_PageInfo pageInfo = _currentTextInfo.pageInfo[pageIndex];
            int firstChar = pageInfo.firstCharacterIndex;
            int lastChar = pageInfo.lastCharacterIndex;

            if (isStartPage)
                firstChar = Mathf.Max(firstChar, _startPosition);

            for (int i = firstChar; i <= lastChar; i++)
            {
                if (session.IsCancelled)
                    yield break;

                OnCharRevealed?.Invoke(i);
                if (session.IsCancelled)
                    yield break;

                char c = i < _currentTextInfo.characterCount
                    ? _currentTextInfo.characterInfo[i].character
                    : '\0';
                OnCharPrinted?.Invoke(c);
                if (session.IsCancelled)
                    yield break;

                float totalPause = _perCharPause + _currentPause;
                _currentPause = 0f;

                _currentVisibleChars = i + 1;
                _currentTextBoxUI.ContentText.maxVisibleCharacters = _currentVisibleChars;

                if (totalPause > 0f)
                    yield return new WaitForSeconds(totalPause);
            }

            if (pageIndex == _currentTextInfo.pageCount - 1)
            {
                OnCharRevealed?.Invoke(_currentTextInfo.characterCount);
                if (session.IsCancelled)
                    yield break;

                if (_currentPause > 0f)
                {
                    yield return new WaitForSeconds(_currentPause);
                    _currentPause = 0f;
                }
            }
        }
    }
}