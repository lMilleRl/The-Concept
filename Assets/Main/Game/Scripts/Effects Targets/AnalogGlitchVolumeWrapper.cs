using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.AnalogGlitch;

public class AnalogGlitchVolumeWrapper : AnalogGlitchBase
{
    [SerializeField] private Volume _volume;

    private AnalogGlitchVolume _glitchVolume;

    private AnalogGlitchVolume GlitchVolume
    {
        get
        {
            if (_glitchVolume == null && _volume != null && _volume.profile != null)
                _volume.profile.TryGet(out _glitchVolume);

            return _glitchVolume;
        }
    }

    public override bool Enabled
    {
        get => _volume != null && _volume.enabled;
        set
        {
            if (_volume != null)
                _volume.enabled = value;
        }
    }

    public override float ScanLineJitter
    {
        get => GlitchVolume != null ? GlitchVolume.scanLineJitter.value : 0f;
        set
        {
            if (GlitchVolume != null)
                GlitchVolume.scanLineJitter.Override(value);
        }
    }

    public override float ColorDrift
    {
        get => GlitchVolume != null ? GlitchVolume.colorDrift.value : 0f;
        set
        {
            if (GlitchVolume != null)
                GlitchVolume.colorDrift.Override(value);
        }
    }

    public override float VerticalJump
    {
        get => GlitchVolume != null ? GlitchVolume.verticalJump.value : 0f;
        set
        {
            if (GlitchVolume != null)
                GlitchVolume.verticalJump.Override(value);
        }
    }

    public override float HorizontalShake
    {
        get => GlitchVolume != null ? GlitchVolume.horizontalShake.value : 0f;
        set
        {
            if (GlitchVolume != null)
                GlitchVolume.horizontalShake.Override(value);
        }
    }
}
