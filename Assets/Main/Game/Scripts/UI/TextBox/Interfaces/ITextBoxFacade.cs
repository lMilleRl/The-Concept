using System;

namespace TextBox
{
    public interface ITextBoxFacade
    {
        event Action OnPageStarted;
        event Action OnPageFinished;
        event Action OnCurrentTextEnded;
        event Action OnHidden;

        void Show(TextBoxData data);
        void ReplaceText(TextBoxData data, int startCharIndex);
        void Hide();
        void TryTurnPage();
    }
}
