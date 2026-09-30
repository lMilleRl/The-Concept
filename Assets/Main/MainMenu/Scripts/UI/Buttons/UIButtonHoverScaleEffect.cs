using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UIButtonMotionChannels))]
public sealed class UIButtonHoverScaleEffect : UIButtonHoverEffectBase
{
    [SerializeField] private float _targetScale = 1.06f;
    [Min(0f)] [SerializeField] private float _duration = 0.12f;

    private UIButtonMotionChannels _motionChannels;
    private float _progress;
    private Tween _tween;

    protected override void Awake()
    {
        base.Awake();
        _motionChannels = GetComponent<UIButtonMotionChannels>();
    }

    protected override void OnActivationZoneEntered()
    {
        Animate(1f);
    }

    protected override void OnActivationZoneExited()
    {
        Animate(0f);
    }

    protected override void OnEffectDisabled()
    {
        _tween?.Kill();
        _progress = 0f;
        _motionChannels.SetHoverScale(1f);
    }

    private void Update()
    {
        _motionChannels.SetHoverScale(Mathf.Lerp(1f, _targetScale, _progress));
    }

    private void Animate(float target)
    {
        _tween?.Kill();
        _tween = DOTween.To(() => _progress, value => _progress = value, target, _duration)
            .SetEase(Ease.OutCubic);
    }
}
