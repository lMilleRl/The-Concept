using DG.Tweening;
using UnityEngine;

public sealed class UIButtonEntranceAlphaEffect : MonoBehaviour
{
    [SerializeField] private UIButtonMotionChannels _motionChannels;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Range(0f, 1f)] [SerializeField] private float _startAlpha;

    private float _originalAlpha;
    private Tween _fadeTween;

    private void Start()
    {
        _originalAlpha = _canvasGroup.alpha;
    }

    private void OnEnable()
    {
        _motionChannels.EntranceStarted += FadeIn;
    }

    private void OnDisable()
    {
        _motionChannels.EntranceStarted -= FadeIn;
        _fadeTween?.Kill();
    }

    private void FadeIn(float duration)
    {
        _fadeTween?.Kill();
        _canvasGroup.alpha = _startAlpha;
        _fadeTween = _canvasGroup.DOFade(_originalAlpha, duration).SetEase(Ease.Linear);
    }
}
