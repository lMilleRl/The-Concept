namespace TextBox
{
    public class EaseSpeedCommand : ITextBoxCommand
    {
        private const int BaseParamCount = 1;

        private readonly ITypeRunner _typeRunner;
        private readonly IDebugWriter _debugWriter;
        private readonly float _defaultTargetSpeed;

        public TextBoxCommandType Type => TextBoxCommandType.EaseSpeed;

        public EaseSpeedCommand(ITypeRunner typeRunner, IDebugWriter debugWriter, float defaultTargetSpeed)
        {
            _typeRunner = typeRunner;
            _debugWriter = debugWriter;
            _defaultTargetSpeed = defaultTargetSpeed;
        }

        public void Execute(TextBoxCommandContext context)
        {
            if (context.Params.Length == 0)
            {
                _typeRunner.SetSpeedEase(_defaultTargetSpeed, context.StartCharIndex, context.CharLength, EaseType.None);
                return;
            }

            float targetSpeed = context.Params[0];
            EaseType ease = context.Params.Length > BaseParamCount
                ? (EaseType)(int)context.Params[^1]
                : EaseType.Linear;

            _typeRunner.SetSpeedEase(targetSpeed, context.StartCharIndex, context.CharLength, ease);
        }
    }
}
