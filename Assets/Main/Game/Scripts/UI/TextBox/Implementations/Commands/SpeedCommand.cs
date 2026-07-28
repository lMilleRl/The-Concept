namespace TextBox
{
    public class SpeedCommand : ITextBoxCommand
    {
        private readonly ITypeRunner _typeRunner;
        private readonly float _defaultCharsPerSecond;

        public TextBoxCommandType Type => TextBoxCommandType.Speed;

        public SpeedCommand(ITypeRunner typeRunner, float defaultCharsPerSecond)
        {
            _typeRunner = typeRunner;
            _defaultCharsPerSecond = defaultCharsPerSecond;
        }

        public void Execute(TextBoxCommandContext context)
        {
            float charsPerSecond = context.Params.Length > 0 ? context.Params[0] : _defaultCharsPerSecond;
            _typeRunner.SetSpeed(charsPerSecond);
        }
    }
}
