using UnityEngine;

namespace TextBox
{
    [CreateAssetMenu(fileName = "New Voice Profile", menuName = "TextBox/Voice Profile")]
    public class VoiceProfile : ScriptableObject
    {
        public AudioClip[] Clips;
        public float Pitch = 1f;
        public float PitchVariation = 0.1f;
        public int CharsPerSound = 1;
    }
}
