using System;

namespace TextBox
{
    public interface ITypeRunner
    {
        event Action OnPageFinished;
        event Action OnTextFinished;
        event Action<int> OnCharRevealed;

        void Run(ITextBoxUI ui);
        void Stop();
        void SetSpeed(float charsPerSecond);
        void SetPause(float seconds);
        void TurnToNextPage();
        int GetCurrentVisibleChars();
    }
}
