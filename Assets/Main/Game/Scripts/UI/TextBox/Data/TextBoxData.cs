
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
        public BoardTransitionContext ShowTransition;
        public BoardTransitionContext HideTransition;
    }
}
