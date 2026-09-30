using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UIButtonMotionChannels))]
public sealed class UIButtonClickImpulseEffect : UIButtonClickEffectBase
{
    [SerializeField] private float _targetScale = 0.8f;
    [Min(0f)] [SerializeField] private float _pressDuration = 0.1f;
    [Min(0f)] [SerializeField] private float _releaseDuration = 0.12f;

    private UIButtonMotionChannels _motionChannels;
    private float _scale = 1f;
    private Tween _tween;

    protected override void Awake()
    {
        base.Awake();
        _motionChannels = GetComponent<UIButtonMotionChannels>();
    }

    protected override void OnActivationZoneClicked()
    {
        _tween?.Kill();
        _scale = 1f;
        _tween = DOTween.Sequence()
            .Append(DOTween.To(() => _scale, value => _scale = value, _targetScale, _pressDuration)
                .SetEase(Ease.OutQuad))
            .Append(DOTween.To(() => _scale, value => _scale = value, 1f, _releaseDuration)
                .SetEase(Ease.OutBack));
    }

    protected override void OnEffectDisabled()
    {
        _tween?.Kill();
        _scale = 1f;
        _motionChannels.SetClickScale(1f);
    }

    private void Update()
    {
        _motionChannels.SetClickScale(_scale);
    }
}
