using DG.Tweening;
using UnityEngine;

public class AudioSourceFader : MonoBehaviour
{
    [SerializeField] [Range(0f, float.MaxValue)] private float _fadeDuration = 1f;
    [SerializeField] private AudioSource _audioSource;

    private float _originalVolume;
    private Tweener _activeFade;

    private void Awake()
    {
        _originalVolume = _audioSource.volume;
    }

    public void FadeIn()
    {
        StartFade(_originalVolume);
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
