using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DIntensityEffect : SmoothValueEffect
{
    [SerializeField] private Light2D _light;
    [SerializeField] [Min(0f)] private float _maxIntensity = 1f;
    [SerializeField] private bool _invert;

    protected override float GetMinValue() => 0f;
    protected override float GetMaxValue() => _maxIntensity;
    protected override float GetCurrentValue() => _light != null ? _light.intensity : 0f;

    protected override void SetValue(float value)
    {
        if (_light == null)
            return;

        _light.intensity = _invert ? _maxIntensity - value : value;
    }
}
