using System;
using UnityEngine;

public class AnimationStepEventSource : MonoBehaviour, IStepEventSource
{
    [SerializeField, Range(0f, 1f)] private float _minClipWeight = 0.5f;

    private Foot _lastEmittedFoot;
    private int _lastEmittedFrame = -1;
    private bool _hasEmitted;

    public event Action<StepEvent> StepPerformed;

    public void OnStep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < _minClipWeight)
            return;

        var foot = (Foot)animationEvent.intParameter;

        if (_hasEmitted && foot == _lastEmittedFoot && Time.frameCount == _lastEmittedFrame)
            return;

        _lastEmittedFoot = foot;
        _lastEmittedFrame = Time.frameCount;
        _hasEmitted = true;

        StepPerformed?.Invoke(new StepEvent(foot));
    }
}
