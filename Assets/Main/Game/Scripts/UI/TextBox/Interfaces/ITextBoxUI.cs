using TMPro;

namespace TextBox
{
    public interface ITextBoxUI
    {
        TMP_Text ContentText { get; }
        void ShowBoard(BoardTransitionContext context);
        void HideBoard(BoardTransitionContext context);
        void SetText(string richText);
        TMP_TextInfo GetTextInfo();
        int GetPageCount();
        void SetPage(int pageIndex);
    }
}
