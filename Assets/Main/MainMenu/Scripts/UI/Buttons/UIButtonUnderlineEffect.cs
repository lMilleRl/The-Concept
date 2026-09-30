using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIButtonUnderlineEffect : UIButtonHoverEffectBase
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Image _line;
    [SerializeField] private float _horizontalPadding = 8f;
    [SerializeField] private float _verticalOffset = -26f;
    [Min(0f)] [SerializeField] private float _duration = 0.18f;

    private RectTransform _lineRect;
    private Tween _tween;

    protected override void Awake()
    {
        base.Awake();
        _lineRect = _line.rectTransform;
        _label.ForceMeshUpdate();
        _lineRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _label.preferredWidth + _horizontalPadding);
        _lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        _lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        _lineRect.pivot = new Vector2(0.5f, 0.5f);
        _lineRect.anchoredPosition = new Vector2(0f, _verticalOffset);
        _line.raycastTarget = false;
        _line.color = _label.color;
        _lineRect.localScale = new Vector3(0f, _lineRect.localScale.y, _lineRect.localScale.z);
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
        _lineRect.localScale = new Vector3(0f, _lineRect.localScale.y, _lineRect.localScale.z);
    }

    private void Animate(float target)
    {
        _tween?.Kill();
        _tween = _lineRect.DOScaleX(target, _duration).SetEase(Ease.OutCubic);
    }
}
