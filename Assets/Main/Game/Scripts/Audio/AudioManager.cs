using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _ambientSource;
    [SerializeField] [Range(0f, 1f)] private float _maxVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float _volumeOnStart = 0f;
    [SerializeField] [Range(0f, float.MaxValue)] private float _volumeFadeOnStart = 3f;

    private void Awake()
    {
        if (_ambientSource == null)
            _ambientSource = GetComponent<AudioSource>();
        _ambientSource.volume = 0f;
        FadeAmbient(_maxVolume, _volumeFadeOnStart);
    }

    public void PlayAmbient(AudioClip clip, bool loop = true)
    {
        if (_ambientSource == null) return;
        
        _ambientSource.clip = clip;
        _ambientSource.loop = loop;
        _ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (_ambientSource != null)
            _ambientSource.Stop();
    }

    public void FadeAmbient(float targetVolumeNormalized, float duration)
    {
        if (_ambientSource == null) return;
        
        float targetVolume = targetVolumeNormalized * _maxVolume;
        DOTween.To(() => _ambientSource.volume, x => _ambientSource.volume = x, targetVolume, duration);
    }

    public void MuteAmbient()
    {
        if (_ambientSource != null)
            _ambientSource.volume = 0f;
    }
}
