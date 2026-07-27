namespace TextBox
{
    public interface ITextChanger
    {
        void AddEffect(TextEffectData effectData);
        void RemoveEffect(TextEffectData effectData);
        void ClearAll();
    }
}
