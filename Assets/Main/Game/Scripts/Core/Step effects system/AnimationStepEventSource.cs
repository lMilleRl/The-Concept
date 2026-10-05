using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStepEventSource : MonoBehaviour, IStepEventSource
{
    [Tooltip("Минимальный интервал между шагами (сек). Отсекает двойные шаги при смене направления")]
    [SerializeField, Min(0f)] private float _minStepInterval = 0.15f;

    private Animator _animator;
    private float _lastStepTime = float.NegativeInfinity;

    public event Action<StepEvent> StepPerformed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnStep(AnimationEvent animationEvent)
    {
        if (!IsFromDominantState(animationEvent))
            return;

        if (Time.time - _lastStepTime < _minStepInterval)
            return;

        _lastStepTime = Time.time;
        StepPerformed?.Invoke(new StepEvent((Foot)animationEvent.intParameter));
    }

    private bool IsFromDominantState(AnimationEvent animationEvent)
    {
        var dominantState = _animator.IsInTransition(0)
            ? _animator.GetNextAnimatorStateInfo(0)
            : _animator.GetCurrentAnimatorStateInfo(0);

        return dominantState.fullPathHash == animationEvent.animatorStateInfo.fullPathHash;
    }
}
