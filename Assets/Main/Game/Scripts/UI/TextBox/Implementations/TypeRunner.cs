using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TypeRunner : ITypeRunner
    {
        private readonly ICoroutineRunner _coroutineRunner;

        private ITextBoxUI _currentTextBoxUI;
        private TMP_TextInfo _currentTextInfo;
        private Coroutine _turnPagesCoroutine;
        private Coroutine _typePageCoroutine;

        private float _perCharPause;
        private float _currentPause;
        private bool _canTurnPage;
        private int _currentVisibleChars;

        public event Action OnPageFinished;
        public event Action OnTextFinished;
        public event Action<int> OnCharRevealed;

        public TypeRunner(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;

            OnPageFinished += () => _isPageFinished = true;
        }

        public void Run(ITextBoxUI ui)
        {
            _currentTextBoxUI = ui;
            _currentTextInfo = _currentTextBoxUI.GetTextInfo();
            _currentVisibleChars = 0;
            _currentTextBoxUI.ContentText.maxVisibleCharacters = 0;
            Stop();
            _turnPagesCoroutine = _coroutineRunner.StartCoroutine(TurnPages());
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
        }

        public void SetSpeed(float charsPerSecond)
        {
            _perCharPause = 1f / charsPerSecond;
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
            int pageCount = _currentTextBoxUI.GetPageCount();

            for (int page = 0; page < pageCount; page++)
            {
                _currentTextBoxUI.SetPage(page + 1);
                _typePageCoroutine = _coroutineRunner.StartCoroutine(TypePage(page));
                yield return _typePageCoroutine;

                OnPageFinished?.Invoke();

                yield return new WaitUntil(() => _canTurnPage);
                _canTurnPage = false;
            }

            OnTextFinished?.Invoke();
        }

        private IEnumerator TypePage(int pageIndex)
        {
            TMP_PageInfo pageInfo = _currentTextInfo.pageInfo[pageIndex];
            int firstChar = pageInfo.firstCharacterIndex;
            int lastChar = pageInfo.lastCharacterIndex;

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
        }
    }
}