using TextBox;
using UnityEngine;

public class GlitchEffectProvider : ProgressiveTargetBase
{
    [SerializeField] private AnalogGlitchBase _analogGlitch;
    [SerializeField] private float _scanLineJitterScale = 1f;
    [SerializeField] private float _colorDriftScale = 0.5f;
    [SerializeField] private float _verticalJumpScale = 0.3f;
    [SerializeField] private float _horizontalShakeScale = 0.2f;

    protected override void UpdateActiveState(float progress)
    {
        if (_analogGlitch == null)
            return;

        _analogGlitch.Enabled = progress > 0f;
    }

    protected override void OnProgress(float progress)
    {
        if (_analogGlitch == null)
        {
            Debug.LogWarning($"[{nameof(GlitchEffectProvider)}] AnalogGlitchBase reference is null.");
            return;
        }

        _analogGlitch.ScanLineJitter = progress * _scanLineJitterScale;
        _analogGlitch.ColorDrift = progress * _colorDriftScale;
        _analogGlitch.VerticalJump = progress * _verticalJumpScale;
        _analogGlitch.HorizontalShake = progress * _horizontalShakeScale;
    }
}
