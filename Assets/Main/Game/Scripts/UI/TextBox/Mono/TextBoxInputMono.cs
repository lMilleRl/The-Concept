using UnityEngine;

namespace TextBox
{
    public class TextBoxInputMono : MonoBehaviour
    {
        private TextBoxInput _input;

        public void Init(TextBoxInput input)
        {
            _input = input;
        }

        private void Update()
        {
            _input?.Tick();
        }
    }
}
