using TMPro;

namespace TextBox
{
    public interface ITextFormChanger
    {
        void SetText(ITextBoxUI ui);
        void AddEffect(TextEffectData effectData);
        void RemoveEffect(TextEffectData effectData);
        void ClearAll();
    }
}
