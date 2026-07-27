using UnityEngine;

namespace TextBox
{
    public interface ITextEffect
    {
        TextBoxCommandType EffectType { get; }
        Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams);
    }
}
