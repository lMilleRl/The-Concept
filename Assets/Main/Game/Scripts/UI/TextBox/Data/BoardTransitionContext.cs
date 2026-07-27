using UnityEngine;

namespace TextBox
{
    [System.Serializable]
    public struct BoardTransitionContext
    {
        public BoardTransitionType Type;
        public float Duration;
        public AnimationCurve Curve;
    }
}
