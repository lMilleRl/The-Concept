using Pixelation.Runtime;
using TextBox;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class PixelationEffectProvider : ProgressiveTargetBase
{
    [SerializeField] private Volume _volume;
    [SerializeField, Min(1f)] private float _maxPixelSize = 32f;

    private PixelationVolume _pixelationVolume;

    private PixelationVolume PixelationVolume
    {
        get
        {
            if (_pixelationVolume == null && _volume != null && _volume.profile != null)
                _volume.profile.TryGet(out _pixelationVolume);

            return _pixelationVolume;
        }
    }

    protected override void UpdateActiveState(float progress)
    {
        if (PixelationVolume != null)
            PixelationVolume.active = progress > 0f;
    }

    protected override void OnProgress(float progress)
    {
        if (PixelationVolume == null)
        {
            Debug.LogWarning($"[{nameof(PixelationEffectProvider)}] PixelationVolume not found on volume profile.");
            return;
        }

        PixelationVolume.progress.Override(progress);
        PixelationVolume.maxPixelSize.Override(_maxPixelSize);
    }
}
