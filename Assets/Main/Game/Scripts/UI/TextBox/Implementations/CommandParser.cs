using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;

namespace TextBox
{
    public class CommandParser : ICommandParser
    {
        private readonly ICommandCoordinator _coordinator;
        private readonly ITypeRunner _typeRunner;

        private List<TextBoxCommandContext> _commands;
        private int _currentCommandIndex;

        public CommandParser(ICommandCoordinator coordinator, ITypeRunner typeRunner)
        {
            _coordinator = coordinator;
            _typeRunner = typeRunner;

            _typeRunner.OnCharRevealed += CheckCommands;
        }

        public void Init(TMP_TextInfo textInfo)
        {
            textInfo.textComponent.ForceMeshUpdate();

            _commands = new List<TextBoxCommandContext>();
            _currentCommandIndex = 0;

            for (int i = 0; i < textInfo.linkCount; i++)
            {
                TMP_LinkInfo linkInfo = textInfo.linkInfo[i];
                string linkID = linkInfo.GetLinkID();

                // TODO: парсить linkID в TextBoxCommandContext
                // Формат: "commandName=param", например "pause=0.5", "shake", "speed=30"
                // 1. Сплит по '=' → commandName и (опционально) paramString
                // 2. Enum.TryParse(commandName) → TextBoxCommandType
                // 3. float.TryParse(paramString) → FloatParam (если есть)
                // 4. Заполнить StartCharIndex = linkInfo.linkTextfirstCharacterIndex
                //    и CharLength = linkInfo.linkTextLength
                // 5. Добавить в _commands

                TextBoxCommandContext context = ParseLinkID(linkID, linkInfo);
                _commands.Add(context);
            }
        }

        public void CheckCommands(int charIndex)
        {
            while (_currentCommandIndex < _commands.Count
                   && charIndex >= _commands[_currentCommandIndex].StartCharIndex)
            {
                TextBoxCommandContext cmd = _commands[_currentCommandIndex];
                _coordinator.ExecuteCommand(cmd.CommandType, cmd);
                _currentCommandIndex++;
            }
        }

        private TextBoxCommandContext ParseLinkID(string linkID, TMP_LinkInfo linkInfo)
        {
            // TODO: реализовать парсинг — это твоя часть
            // Подсказка:
            // string[] parts = linkID.Split('=');
            // string commandName = parts[0];
            // string paramBlock = parts.Length > 1 ? parts[1] : "";
            // float[] params = ParseParams(paramBlock);
            // System.Enum.TryParse(commandName, true, out TextBoxCommandType type);

            var paramsAndCmdParts = linkID.Split('=');

            string commandName = paramsAndCmdParts[0];
            if (!Enum.TryParse(commandName, true, out TextBoxCommandType cmdType))
            {
                cmdType = TextBoxCommandType.Unknown;
            }

            string paramBlock = paramsAndCmdParts.Length > 1 ? paramsAndCmdParts[1] : "";
            var cmdParams = ParseParams(paramBlock);

            return new TextBoxCommandContext
            {
                CommandType = cmdType,
                StartCharIndex = linkInfo.linkTextfirstCharacterIndex,
                CharLength = linkInfo.linkTextLength,
                Params = cmdParams
            };
        }

        private float[] ParseParams(string paramBlock)
        {
            if (string.IsNullOrEmpty(paramBlock))
                return Array.Empty<float>();

            string[] parts = paramBlock.Split(':');
            float[] result = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                float.TryParse(parts[i], NumberStyles.Float,
                    CultureInfo.InvariantCulture, out result[i]);

            return result;
        }
    }
}