namespace TextBox
{
    public class EaseSpeedCommand : ITextBoxCommand
    {
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
            float targetSpeed = context.Params.Length > 0 ? context.Params[0] : _defaultTargetSpeed;
            EaseType ease = context.Params.Length > 1
                ? (EaseType)(int)context.Params[1]
                : EaseType.Linear;

            _debugWriter.Log($"[EaseSpeed] Execute at {context.StartCharIndex}, len {context.CharLength}, target {targetSpeed}, ease {ease}");
            _typeRunner.SetSpeedEase(targetSpeed, context.StartCharIndex, context.CharLength, ease);
        }
    }
}
