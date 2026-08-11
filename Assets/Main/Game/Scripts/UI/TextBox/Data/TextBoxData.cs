using TMPro;
using UnityEngine;

namespace TextBox
{
    [System.Serializable]
    public class TextBoxData
    {
        public string Text;
        public VoiceProfile Voice;
        public bool AutoPlay;
        public float AutoPagePause;
        public TextStyleProfile Style;
        public BoardTransitionContext ShowTransition;
        public BoardTransitionContext HideTransition;
    }
}
