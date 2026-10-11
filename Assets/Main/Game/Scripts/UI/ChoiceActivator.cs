using UnityEngine;

namespace Choices
{
    public class ChoiceActivator : MonoBehaviour, global::IInteractable
    {
        [SerializeField] private ChoiceCompositionRoot _choiceCompositionRoot;
        [SerializeField] private ChoiceSequenceData _choiceSequence;
        [SerializeField] private ChoiceOptionEventBinding[] _optionEventBindings;

        public void Activate() => OpenChoice();

        public void OpenChoice()
        {
            if (_choiceCompositionRoot == null || _choiceSequence == null)
            {
                Debug.LogError($"[{nameof(ChoiceActivator)}] Assign a choice composition root and sequence.", this);
                return;
            }

            _choiceCompositionRoot.OpenChoice(_choiceSequence, _optionEventBindings);
        }
    }
}