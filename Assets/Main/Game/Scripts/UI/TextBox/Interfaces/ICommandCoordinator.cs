namespace TextBox
{
    public interface ICommandCoordinator
    {
        void ExecuteCommand(TextBoxCommandType type, TextBoxCommandContext context);
    }
}
