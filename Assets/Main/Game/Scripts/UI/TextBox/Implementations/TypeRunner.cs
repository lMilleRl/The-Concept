using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TypeRunner : ITypeRunner, IDisposable
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IDebugWriter _debugWriter;

        private ITextBoxUI _currentTextBoxUI;
        private TMP_TextInfo _currentTextInfo;
        private Coroutine _turnPagesCoroutine;
        private Coroutine _typePageCoroutine;
        private Coroutine _easeSpeedCoroutine;

        private float _perCharPause;
        private float _currentPause;
        private bool _canTurnPage;
        private int _currentVisibleChars;
        private int _startPosition;

        public event Action OnPageFinished;
        public event Action OnTextFinished;
        public event Action<int> OnCharRevealed;

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

            _turnPagesCoroutine = _coroutineRunner.StartCoroutine(TurnPages());
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
                float t = Mathf.Clamp01((float)(_currentVisibleChars - 1 - startCharIndex) / length);
                _perCharPause = Mathf.Lerp(startPause, targetPause, ApplyEase(t, ease));
                yield return null;
            }

            _perCharPause = targetPause;
            _easeSpeedCoroutine = null;
        }

        private float ApplyEase(float t, EaseType ease)
        {
            return ease switch
            {
                EaseType.Linear => t,
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

        private IEnumerator TurnPages()
        {
            yield return new WaitUntil(() => _currentTextBoxUI.IsTextInitialized);

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
                if (page != startPage)
                {
                    int firstChar = _currentTextInfo.pageInfo[page].firstCharacterIndex;
                    _currentTextBoxUI.ContentText.maxVisibleCharacters = firstChar;
                    _currentTextBoxUI.ContentText.pageToDisplay = page + 1;
                }

                _typePageCoroutine = _coroutineRunner.StartCoroutine(TypePage(page, page == startPage));
                yield return _typePageCoroutine;

                OnPageFinished?.Invoke();

                yield return new WaitUntil(() => _canTurnPage);
                _canTurnPage = false;
            }

            OnTextFinished?.Invoke();
        }

        private IEnumerator TypePage(int pageIndex, bool isStartPage)
        {
            TMP_PageInfo pageInfo = _currentTextInfo.pageInfo[pageIndex];
            int firstChar = pageInfo.firstCharacterIndex;
            int lastChar = pageInfo.lastCharacterIndex;

            if (isStartPage)
                firstChar = Mathf.Max(firstChar, _startPosition);

            for (int i = firstChar; i <= lastChar; i++)
            {
                _currentVisibleChars = i + 1;
                _currentTextBoxUI.ContentText.maxVisibleCharacters = _currentVisibleChars;

                OnCharRevealed?.Invoke(i);

                float totalPause = _perCharPause + _currentPause;
                _currentPause = 0f;

                if (totalPause > 0f)
                    yield return new WaitForSeconds(totalPause);
            }

            if (pageIndex == _currentTextInfo.pageCount - 1)
                OnCharRevealed?.Invoke(_currentTextInfo.characterCount);
        }
    }
}