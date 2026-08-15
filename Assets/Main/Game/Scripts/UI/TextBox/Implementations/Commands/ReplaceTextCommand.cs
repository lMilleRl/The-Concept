namespace TextBox
{
    public class ReplaceTextCommand : ITextBoxCommand
    {
        private readonly ITextBoxFacade _facade;
        private readonly ITextRegistry _textRegistry;
        private readonly IDebugWriter _debugWriter;

        public TextBoxCommandType Type => TextBoxCommandType.ReplaceText;

        public ReplaceTextCommand(ITextBoxFacade facade, ITextRegistry textRegistry, IDebugWriter debugWriter)
        {
            _facade = facade;
            _textRegistry = textRegistry;
            _debugWriter = debugWriter;
        }

        public void Execute(TextBoxCommandContext context)
        {
            if (context.Params.Length == 0)
            {
                _debugWriter.LogWarning($"[{nameof(ReplaceTextCommand)}] Missing text ID parameter.");
                return;
            }

            TextId textId = (TextId)(int)context.Params[0];
            int startCharIndex = context.Params.Length > 1 ? (int)context.Params[1] : 0;

            TextBoxData data = _textRegistry.Get(textId);

            if (data == null)
            {
                _debugWriter.LogWarning($"[{nameof(ReplaceTextCommand)}] No text found for ID {textId}.");
                return;
            }

            _facade.ReplaceText(data, startCharIndex);
        }
    }
}
