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
        private bool _wasReinitialized;

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
            _wasReinitialized = true;
            return result;
        }

        public void CheckCommands(int charIndex)
        {
            _wasReinitialized = false;

            try
            {
                while (_currentCommandIndex < _commands.Length
                       && charIndex >= _commands[_currentCommandIndex].StartCharIndex)
                {
                    TextBoxCommandContext cmd = _commands[_currentCommandIndex++];
                    _coordinator.ExecuteCommand(cmd.CommandType, cmd);
                    if (_wasReinitialized)
                        return;
                }
            }
            finally
            {
                _wasReinitialized = false;
            }
        }
    }
}