namespace TextBox
{
    public interface ICommandParser
    {
        ParseResult Init(string rawText);
        void CheckCommands(int currentVisibleChars);
    }
}
