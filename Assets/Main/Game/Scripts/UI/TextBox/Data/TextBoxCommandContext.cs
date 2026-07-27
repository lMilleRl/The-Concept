namespace TextBox
{
    public struct TextBoxCommandContext
    {
        public TextBoxCommandType CommandType;
        public int StartCharIndex;
        public int CharLength;
        public float[] Params;
    }
}
