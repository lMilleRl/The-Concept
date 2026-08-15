using System;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TextBoxUI : ITextBoxUI, IDisposable
    {
        private readonly TMP_Text _text;
        private readonly ITextBoxBoard _board;

        private const int NoPendingPosition = -1;

        private bool _isSubscribed;
        private int _pendingCharIndex = NoPendingPosition;
        private bool _isTextInitialized;

        public TMP_Text ContentText => _text;
        public Canvas Canvas => _text.GetComponentInParent<Canvas>();

        public event Action<TMP_TextInfo> OnTextMeshUpdated;

        public bool IsTextInitialized => _isTextInitialized;

        public TextBoxUI(TMP_Text text, ITextBoxBoard board)
        {
            _text = text;
            _board = board;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTMPSystemTextChanged);
            _isSubscribed = true;
        }

        public void Dispose()
        {
            if (_isSubscribed)
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTMPSystemTextChanged);
                _isSubscribed = false;
            }
        }

        public void ShowBoard(BoardTransitionContext context)
        {
            _board.Show(context);
        }

        public void HideBoard(BoardTransitionContext context)
        {
            _board.Hide(context);
        }

        public void InitText(string richText, int startPosition)
        {
            _isTextInitialized = false;
            _pendingCharIndex = Mathf.Max(0, startPosition);
            _text.maxVisibleCharacters = 0;
            _text.overflowMode = TextOverflowModes.Page;
            _text.text = richText;
        }

        public int GetPageIndexForChar(int charIndex)
        {
            TMP_TextInfo textInfo = _text.textInfo;
            if (textInfo == null || textInfo.pageCount == 0)
                return 0;

            for (int i = 0; i < textInfo.pageCount; i++)
            {
                TMP_PageInfo pageInfo = textInfo.pageInfo[i];
                if (charIndex >= pageInfo.firstCharacterIndex && charIndex <= pageInfo.lastCharacterIndex)
                    return i;
            }

            return 0;
        }

        private void SetPosition(int charIndex)
        {
            int clamped = Mathf.Max(0, charIndex);
            _text.maxVisibleCharacters = clamped;

            TMP_TextInfo textInfo = _text.textInfo;
            if (textInfo == null || textInfo.pageCount == 0)
            {
                _text.pageToDisplay = 1;
                return;
            }

            _text.pageToDisplay = GetPageIndexForChar(clamped) + 1;
        }

        public TMP_TextInfo GetTextInfo()
        {
            return _text.textInfo;
        }

        public int GetPageCount()
        {
            return _text.textInfo != null ? _text.textInfo.pageCount : 0;
        }

        private void OnTMPSystemTextChanged(UnityEngine.Object obj)
        {
            if (obj != _text)
                return;

            TMP_TextInfo textInfo = _text.textInfo;
            if (textInfo == null)
                return;

            OnTextMeshUpdated?.Invoke(textInfo);

            if (_pendingCharIndex < 0)
                return;

            int clamped = Mathf.Min(_pendingCharIndex, textInfo.characterCount);
            SetPosition(clamped);
            _pendingCharIndex = NoPendingPosition;
            _isTextInitialized = true;
        }
    }
}
