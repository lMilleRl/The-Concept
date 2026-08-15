using System.Collections.Generic;

namespace TextBox
{
    public class CommandCoordinator : ICommandCoordinator
    {
        private readonly Dictionary<TextBoxCommandType, ITextBoxCommand> _commands;

        public CommandCoordinator(ITextBoxCommand[] commands)
        {
            _commands = new Dictionary<TextBoxCommandType, ITextBoxCommand>(commands.Length);

            foreach (var command in commands)
                _commands.TryAdd(command.Type, command);
        }

        public void Register(ITextBoxCommand command)
        {
            _commands.TryAdd(command.Type, command);
        }

        public void ExecuteCommand(TextBoxCommandType type, TextBoxCommandContext context)
        {
            if (_commands.TryGetValue(type, out ITextBoxCommand command))
                command.Execute(context);
        }
    }
}
