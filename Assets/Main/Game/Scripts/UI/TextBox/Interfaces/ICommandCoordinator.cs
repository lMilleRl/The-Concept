namespace TextBox
{
    public interface ICommandCoordinator
    {
        void Register(ITextBoxCommand command);
        void ExecuteCommand(TextBoxCommandType type, TextBoxCommandContext context);
    }
}
