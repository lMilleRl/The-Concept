namespace TextBox
{
    public class EffectCommand : ITextBoxCommand
    {
        private readonly TextBoxCommandType _type;
        private readonly ITextChanger _textChanger;

        public TextBoxCommandType Type => _type;

        public EffectCommand(TextBoxCommandType type, ITextChanger textChanger)
        {
            _type = type;
            _textChanger = textChanger;
        }

        public void Execute(TextBoxCommandContext context)
        {
            var effectData = new TextEffectData
            {
                EffectType = context.CommandType,
                StartCharIndex = context.StartCharIndex,
                CharLength = context.CharLength,
                Params = context.Params
            };

            _textChanger.AddEffect(effectData);
        }
    }
}
