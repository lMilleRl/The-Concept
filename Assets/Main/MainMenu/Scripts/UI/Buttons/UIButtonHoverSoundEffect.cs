using UnityEngine;

public sealed class UIButtonHoverSoundEffect : UIButtonHoverEffectBase
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hoverClip;

    protected override void OnActivationZoneEntered()
    {
        _audioSource.PlayOneShot(_hoverClip);
    }

    protected override void OnActivationZoneExited()
    {
    }
}
