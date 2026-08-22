using TextBox;
using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.TVShutdown;

public class TVShutdownEffectProvider : ProgressiveTargetBase
{
    [SerializeField] private Volume _volume;
    [SerializeField] private float _maxFlashIntensity = 0.8f;
    [SerializeField] private Color _flashColor = Color.white;

    private TVShutdownVolume _tvVolume;

    private TVShutdownVolume TVVolume
    {
        get
        {
            if (_tvVolume == null && _volume != null && _volume.profile != null)
                _volume.profile.TryGet(out _tvVolume);

            return _tvVolume;
        }
    }

    protected override void UpdateActiveState(float progress)
    {
        if (TVVolume != null)
            TVVolume.active = progress > 0f;
    }

    protected override void OnProgress(float progress)
    {
        if (TVVolume == null)
        {
            Debug.LogWarning($"[{nameof(TVShutdownEffectProvider)}] TVShutdownVolume not found on volume profile.");
            return;
        }

        TVVolume.progress.Override(progress);
        TVVolume.flashIntensity.Override(_maxFlashIntensity);
        TVVolume.flashColor.Override(_flashColor);
    }
}
