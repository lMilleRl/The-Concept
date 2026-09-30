using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIButtonColorCycleEffect : UIButtonHoverEffectBase
{
    [SerializeField] private Color _originalColor = Color.white ;
    [SerializeField] private MaskableGraphic[] _graphics;
    [Min(0f)] [SerializeField] private float _cycleSpeed = 5f;
    [Min(0f)] [SerializeField] private float _restoreDuration = 0.12f;

    private bool _isHovered;
    private List<Tween> _restoreTweens;

    private void Awake()
    {
        base.Awake();
        _restoreTweens = new List<Tween>();
    }
    
    protected override void OnActivationZoneEntered()
    {
        foreach (var tw in _restoreTweens)
            tw?.Kill();
        _isHovered = true;
    }

    protected override void OnActivationZoneExited()
    {
        _isHovered = false;
        foreach (var tw in _restoreTweens)
            tw?.Kill();
        foreach (var g in _graphics)
            _restoreTweens.Add(g.DOColor(_originalColor, _restoreDuration));
    }

    protected override void OnEffectDisabled()
    {
        foreach (var tw in _restoreTweens)
            tw?.Kill();
        _isHovered = false;
        foreach (var g in _graphics)
            g.color = _originalColor;
    }

    private void Update()
    {
        if (!_isHovered) return;

        float time = Time.time * _cycleSpeed;
        var resultColor = new Color(
            0.85f + Mathf.Sin(time) * 0.15f,
            0.7f + Mathf.Sin(time + 2.1f) * 0.15f,
            0.9f + Mathf.Sin(time + 1.2f) * 0.1f,
            1f);
        foreach (var g in _graphics)
            g.color = resultColor;
    }
}
