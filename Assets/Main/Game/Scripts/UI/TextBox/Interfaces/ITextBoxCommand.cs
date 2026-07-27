namespace TextBox
{
    public interface ITextBoxCommand
    {
        TextBoxCommandType Type { get; }
        void Execute(TextBoxCommandContext context);
    }
}
