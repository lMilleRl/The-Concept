using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DSpotRadiusEffect : SmoothValueEffect
{
    [SerializeField] private Light2D _light;
    [SerializeField] [Min(0f)] private float _maxRadius = 1f;
    [SerializeField] private bool _invert;

    protected override float GetMinValue() => _light != null ? _light.pointLightOuterRadius : 0f;
    protected override float GetMaxValue() => _maxRadius;
    protected override float GetCurrentValue() => _light != null ? _light.pointLightOuterRadius : 0f;

    protected override void SetValue(float value)
    {
        if (_light == null)
            return;

        _light.pointLightOuterRadius = _invert ? _maxRadius - value : value;
    }
}
