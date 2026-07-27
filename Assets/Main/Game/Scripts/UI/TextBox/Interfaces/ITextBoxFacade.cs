namespace TextBox
{
    public interface ITextBoxFacade
    {
        void Show(TextBoxData data);
        void Hide();
        void TryTurnPage();
    }
}
