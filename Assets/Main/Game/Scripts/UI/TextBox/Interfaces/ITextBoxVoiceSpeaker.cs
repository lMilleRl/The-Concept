namespace TextBox
{
    public interface ITextBoxVoiceSpeaker
    {
        void SetProfile(VoiceProfile profile);
        void PlayChar(char c);
        void Mute();
        void Resume();
    }
}
