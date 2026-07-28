using UnityEngine;

namespace TextBox
{
    public class TextChangerMono : MonoBehaviour
    {
        private TextFormChanger _textFormChanger;

        public TextFormChanger TextFormChanger => _textFormChanger;

        public void Init(TextFormChanger textFormChanger)
        {
            _textFormChanger = textFormChanger;
        }

        private void Update()
        {
            _textFormChanger?.Tick();
        }
    }
}
