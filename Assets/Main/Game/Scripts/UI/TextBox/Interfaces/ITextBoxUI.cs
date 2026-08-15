using System;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public interface ITextBoxUI : IDisposable
    {
        TMP_Text ContentText { get; }
        Canvas Canvas { get; }
        bool IsTextInitialized { get; }
        void ShowBoard(BoardTransitionContext context);
        void HideBoard(BoardTransitionContext context);
        void InitText(string richText, int startPosition);
        int GetPageIndexForChar(int charIndex);
        TMP_TextInfo GetTextInfo();
        int GetPageCount();
    }
}
