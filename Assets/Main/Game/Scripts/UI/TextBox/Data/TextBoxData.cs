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
        public float DefaultSpeed = 20f;
        public Color DefaultColor = Color.white;
        public float DefaultFontSize = 36f;
        public TMP_FontAsset DefaultFont;
        public BoardTransitionContext ShowTransition;
        public BoardTransitionContext HideTransition;
    }
}
