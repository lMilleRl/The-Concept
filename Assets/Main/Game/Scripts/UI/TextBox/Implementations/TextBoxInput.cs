using System;
using UnityEngine;

namespace TextBox
{
    public class TextBoxInput : ITextBoxInput
    {
        public event Action OnTurnPagePressed;

        private readonly KeyCode _turnPageKey;
        private bool _enabled;

        public TextBoxInput(KeyCode turnPageKey)
        {
            _turnPageKey = turnPageKey;
        }

        public void Enable() => _enabled = true;
        public void Disable() => _enabled = false;

        public void Tick()
        {
            if (_enabled && Input.GetKeyDown(_turnPageKey))
            {
                OnTurnPagePressed?.Invoke();
            }
        }
    }
}
