using TextBox;
using UnityEngine;
using UnityEngine.UI;

public class WhiteNoiseEffectProvider : ProgressiveTargetBase
{
    [SerializeField] private RawImage _noiseImage;
    [SerializeField, Range(0f, 1f)] private float _maxIntensity = 1f;
    [SerializeField] private AudioSource _noiseAudio;

    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");

    protected override void UpdateActiveState(float progress)
    {
        if (_noiseImage != null)
            _noiseImage.enabled = progress > 0f;

        if (_noiseAudio != null)
        {
            if (progress > 0f && !_noiseAudio.isPlaying)
                _noiseAudio.Play();
            else if (progress <= 0f && _noiseAudio.isPlaying)
                _noiseAudio.Stop();
        }
    }

    protected override void OnProgress(float progress)
    {
        float intensity = progress * _maxIntensity;

        if (_noiseImage != null)
        {
            Material mat = _noiseImage.material;
            if (mat != null)
                mat.SetFloat(IntensityId, intensity);
            else
                Debug.LogWarning($"[{nameof(WhiteNoiseEffectProvider)}] Material is null.");
        }
        else
        {
            Debug.LogWarning($"[{nameof(WhiteNoiseEffectProvider)}] Noise Image reference is null.");
        }

        if (_noiseAudio != null)
            _noiseAudio.volume = intensity;
    }
}
