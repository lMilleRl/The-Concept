using DG.Tweening;
using UnityEngine;

public class AudioSourceFader : MonoBehaviour
{
    [SerializeField] [Range(0f, float.MaxValue)] private float _fadeDuration = 1f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _isToTargetFade;
    [SerializeField] [Range(0f, float.MaxValue)] private float _targetVolume = 1f;

    private float _originalVolume;
    private Tweener _activeFade;

    private void Awake()
    {
        _originalVolume = _audioSource.volume;
    }

    public void FadeIn()
    {
        var targetVolume = _isToTargetFade ? _targetVolume : _originalVolume; 
        StartFade(targetVolume);
    }

    public void FadeOut()
    {
        StartFade(0f);
    }

    public void RestoreVolume()
    {
        _audioSource.volume = _originalVolume;
    }

    private void StartFade(float targetVolume)
    {
        _activeFade?.Kill();
        _activeFade = _audioSource.DOFade(targetVolume, _fadeDuration);
    }
}
