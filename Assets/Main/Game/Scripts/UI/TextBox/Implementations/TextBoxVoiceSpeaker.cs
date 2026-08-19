using UnityEngine;

namespace TextBox
{
    public class TextBoxVoiceSpeaker : ITextBoxVoiceSpeaker, ICharPrintedListener
    {
        private readonly AudioSource _audioSource;

        private VoiceProfile _profile;
        private bool _muted;
        private int _charCounter;

        public TextBoxVoiceSpeaker(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public void SetProfile(VoiceProfile profile)
        {
            _profile = profile;
            _charCounter = 0;
        }

        public void PlayChar(char c)
        {
            if (_muted || _profile == null || _profile.Clips.Length == 0 || !char.IsLetterOrDigit(c))
                return;

            if (_charCounter++ < _profile.CharsPerSound)
                return;

            _charCounter = 0;

            AudioClip clip = _profile.Clips[Random.Range(0, _profile.Clips.Length)];
            _audioSource.pitch = _profile.Pitch + Random.Range(-_profile.PitchVariation, _profile.PitchVariation);
            _audioSource.PlayOneShot(clip);
        }

        public void OnCharPrinted(char c) => PlayChar(c);

        public void Mute()
        {
            _muted = true;
            _audioSource.Stop();
        }

        public void Resume()
        {
            _muted = false;
            _charCounter = 0;
        }
    }
}
