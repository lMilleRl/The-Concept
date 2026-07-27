using UnityEngine;

namespace TextBox
{
    public interface IBoardTransition
    {
        BoardTransitionType Type { get; }
        void Play(GameObject target, BoardTransitionContext context, bool reverse, System.Action onComplete = null);
    }
}
