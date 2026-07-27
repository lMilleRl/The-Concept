using TMPro;

namespace TextBox
{
    public interface ICommandParser
    {
        void Init(TMP_TextInfo textInfo);
        void CheckCommands(int currentVisibleChars);
    }
}
