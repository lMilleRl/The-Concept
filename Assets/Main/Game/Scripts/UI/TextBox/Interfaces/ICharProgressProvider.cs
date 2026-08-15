namespace TextBox
{
    public interface ICharProgressProvider
    {
        float GetProgress(int charIndex, int startCharIndex, int charLength, EaseType ease);
    }
}
