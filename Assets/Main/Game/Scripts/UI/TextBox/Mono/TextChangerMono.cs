using UnityEngine;

namespace TextBox
{
    public class TextChangerMono : MonoBehaviour
    {
        private TextChanger _textChanger;

        public TextChanger TextChanger => _textChanger;

        public void Init(TextChanger textChanger)
        {
            _textChanger = textChanger;
        }

        private void Update()
        {
            _textChanger?.Tick();
        }
    }
}
