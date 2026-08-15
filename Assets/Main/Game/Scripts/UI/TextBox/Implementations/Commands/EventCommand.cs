namespace TextBox
{
    public class EventCommand : ITextBoxCommand
    {
        private readonly IEventRegistry _eventRegistry;
        private readonly IDebugWriter _debugWriter;

        public TextBoxCommandType Type => TextBoxCommandType.Event;

        public EventCommand(IEventRegistry eventRegistry, IDebugWriter debugWriter)
        {
            _eventRegistry = eventRegistry;
            _debugWriter = debugWriter;
        }

        public void Execute(TextBoxCommandContext context)
        {
            if (context.Params.Length == 0)
            {
                _debugWriter.LogWarning($"[{nameof(EventCommand)}] Missing event ID parameter.");
                return;
            }

            int eventId = (int)context.Params[0];
            _eventRegistry.Invoke(eventId);
        }
    }
}
