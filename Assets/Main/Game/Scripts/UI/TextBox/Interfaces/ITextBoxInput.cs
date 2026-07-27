using System;

namespace TextBox
{
    public interface ITextBoxInput
    {
        event Action OnTurnPagePressed;
        void Enable();
        void Disable();
    }
}
