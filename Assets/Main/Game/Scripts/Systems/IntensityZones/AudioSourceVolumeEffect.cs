using UnityEngine;

public class AudioSourceVolumeEffect : SmoothValueEffect
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] [Min(0f)] private float _maxVolume = 1f;
    [SerializeField] private bool _invert;

    protected override float GetMinValue() => 0f;
    protected override float GetMaxValue() => _maxVolume;
    protected override float GetCurrentValue() => _audioSource != null ? _audioSource.volume : 0f;

    protected override void SetValue(float value)
    {
        if (_audioSource == null)
            return;

        _audioSource.volume = _invert ? _maxVolume - value : value;
    }
}
