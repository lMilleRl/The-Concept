namespace TextBox
{
    public class EffectCommand : ITextBoxCommand
    {
        private readonly TextBoxCommandType _type;
        private readonly ITextFormChanger _textFormChanger;

        public TextBoxCommandType Type => _type;

        public EffectCommand(TextBoxCommandType type, ITextFormChanger textFormChanger)
        {
            _type = type;
            _textFormChanger = textFormChanger;
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

            _textFormChanger.AddEffect(effectData);
        }
    }
}
