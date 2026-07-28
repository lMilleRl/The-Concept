namespace TextBox
{
    public class PauseCommand : ITextBoxCommand
    {
        private readonly ITypeRunner _typeRunner;
        private readonly float _defaultSeconds;

        public TextBoxCommandType Type => TextBoxCommandType.Pause;

        public PauseCommand(ITypeRunner typeRunner, float defaultSeconds)
        {
            _typeRunner = typeRunner;
            _defaultSeconds = defaultSeconds;
        }

        public void Execute(TextBoxCommandContext context)
        {
            float seconds = context.Params.Length > 0 ? context.Params[0] : _defaultSeconds;
            _typeRunner.SetPause(seconds);
        }
    }
}
