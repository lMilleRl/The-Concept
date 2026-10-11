using System.Collections;
using TextBox;

namespace Choices
{
    public interface IChoiceTextBoxPresenter
    {
        IEnumerator ShowAndWait(TextBoxData data);
    }

    public interface IChoiceQuestionPrinter
    {
        IEnumerator Print(TextBoxData data);
        void Clear();
    }
}