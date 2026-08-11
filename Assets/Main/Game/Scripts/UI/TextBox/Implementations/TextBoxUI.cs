using TMPro;

namespace TextBox
{
    public class TextBoxUI : ITextBoxUI
    {
        private readonly TMP_Text _text;
        private readonly ITextBoxBoard _board;

        public TMP_Text ContentText => _text;

        public TextBoxUI(TMP_Text text, ITextBoxBoard board)
        {
            _text = text;
            _board = board;
        }

        public void ShowBoard(BoardTransitionContext context)
        {
            _board.Show(context);
        }

        public void HideBoard(BoardTransitionContext context)
        {
            _board.Hide(context);
        }

        public void SetText(string richText)
        {
            _text.text = richText;
            _text.overflowMode = TextOverflowModes.Page;
            _text.ForceMeshUpdate();
        }

        public TMP_TextInfo GetTextInfo()
        {
            return _text.textInfo;
        }

        public int GetPageCount()
        {
            return _text.textInfo.pageCount;
        }

        public void SetPage(int pageIndex)
        {
            _text.pageToDisplay = pageIndex;
            _text.ForceMeshUpdate();
        }
    }
}
