using System.Collections.Generic;
using UnityEngine;

namespace TextBox
{
    public class TextBoxBoard : MonoBehaviour, ITextBoxBoard
    {
        [SerializeField] private GameObject _boardRoot;

        private Dictionary<BoardTransitionType, IBoardTransition> _transitions;

        public void Init(IBoardTransition[] transitions)
        {
            _transitions = new Dictionary<BoardTransitionType, IBoardTransition>(transitions.Length);

            foreach (var transition in transitions)
                _transitions.TryAdd(transition.Type, transition);
        }

        public void Show(BoardTransitionContext context)
        {
            _boardRoot.SetActive(true);

            if (_transitions != null && _transitions.TryGetValue(context.Type, out var transition))
                transition.Play(_boardRoot, context, false);
        }

        public void Hide(BoardTransitionContext context)
        {
            if (_transitions != null && _transitions.TryGetValue(context.Type, out var transition))
                transition.Play(_boardRoot, context, true, () => _boardRoot.SetActive(false));
            else
                _boardRoot.SetActive(false);
        }
    }
}
