using System;

namespace TextBox
{
    public class CommandParser : ICommandParser, IDisposable
    {
        private readonly ICommandCoordinator _coordinator;
        private readonly ITypeRunner _typeRunner;
        private readonly ITagParser _tagParser;

        private TextBoxCommandContext[] _commands;
        private int _currentCommandIndex;

        public CommandParser(ICommandCoordinator coordinator, ITypeRunner typeRunner, ITagParser tagParser)
        {
            _coordinator = coordinator;
            _typeRunner = typeRunner;
            _tagParser = tagParser;

            _typeRunner.OnCharRevealed += CheckCommands;
        }

        public void Dispose()
        {
            _typeRunner.OnCharRevealed -= CheckCommands;
        }

        public ParseResult Init(string rawText)
        {
            var result = _tagParser.Parse(rawText);
            _commands = result.Commands;
            _currentCommandIndex = 0;
            return result;
        }

        public void CheckCommands(int charIndex)
        {
            while (_currentCommandIndex < _commands.Length
                   && charIndex >= _commands[_currentCommandIndex].StartCharIndex)
            {
                TextBoxCommandContext cmd = _commands[_currentCommandIndex];
                _coordinator.ExecuteCommand(cmd.CommandType, cmd);
                _currentCommandIndex++;
            }
        }
    }
}