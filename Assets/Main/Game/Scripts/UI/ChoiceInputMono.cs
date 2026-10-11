using UnityEngine;

namespace Choices
{
    public class ChoiceInputMono : MonoBehaviour
    {
        [SerializeField] private KeyCode _upKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode _downKey = KeyCode.DownArrow;
        [SerializeField] private KeyCode _leftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode _rightKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode _submitKey = KeyCode.Z;
        [SerializeField] private KeyCode _cancelKey = KeyCode.Escape;

        private ChoiceCompositionRoot _choiceRoot;
        private bool _isEnabled;

        public void Init(ChoiceCompositionRoot choiceRoot)
        {
            _choiceRoot = choiceRoot;
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        private void Update()
        {
            if (!_isEnabled || _choiceRoot == null)
                return;

            if (Input.GetKeyDown(_upKey))
                _choiceRoot.MoveSelection(Vector2Int.up);
            else if (Input.GetKeyDown(_downKey))
                _choiceRoot.MoveSelection(Vector2Int.down);
            else if (Input.GetKeyDown(_leftKey))
                _choiceRoot.MoveSelection(Vector2Int.left);
            else if (Input.GetKeyDown(_rightKey))
                _choiceRoot.MoveSelection(Vector2Int.right);
            else if (Input.GetKeyDown(_submitKey))
                _choiceRoot.SubmitSelection();
            else if (Input.GetKeyDown(_cancelKey))
                _choiceRoot.CancelSelection();
        }
    }
}