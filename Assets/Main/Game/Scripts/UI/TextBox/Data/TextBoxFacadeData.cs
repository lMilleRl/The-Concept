namespace TextBox
{
    public struct TextBoxFacadeData
    {
        public ITextBoxUI UI;
        public ITypeRunner TypeRunner;
        public ICommandParser CommandParser;
        public ITextBoxVoiceSpeaker VoiceSpeaker;
        public ITextFormChanger TextFormChanger;
        public ITextBoxInput Input;
        public ICoroutineRunner CoroutineRunner;
        public TextStyleProfile DefaultStyle;
    }
}
