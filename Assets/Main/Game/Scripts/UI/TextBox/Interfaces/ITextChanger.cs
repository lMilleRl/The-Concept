using TMPro;

namespace TextBox
{
    public interface ITextChanger
    {
        void SetText(TMP_Text text);
        void AddEffect(TextEffectData effectData);
        void RemoveEffect(TextEffectData effectData);
        void ClearAll();
    }
}
