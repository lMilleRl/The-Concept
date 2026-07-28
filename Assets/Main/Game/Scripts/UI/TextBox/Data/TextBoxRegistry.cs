using UnityEngine;

namespace TextBox
{
    [CreateAssetMenu(fileName = "New TextBox Registry", menuName = "TextBox/Registry")]
    public class TextBoxRegistry : ScriptableObject
    {
        public TextEffectEntry[] Effects;
        public TextCommandEntry[] Commands;
        public BoardTransitionEntry[] Transitions;
    }
}
