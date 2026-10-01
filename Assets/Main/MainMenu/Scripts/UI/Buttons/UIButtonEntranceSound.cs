using UnityEngine;

[RequireComponent(typeof(UIButtonMotionChannels), typeof(AudioSource))]
public sealed class UIButtonEntranceSound : MonoBehaviour
{
    [SerializeField] private UIButtonMotionChannels _motionChannels;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;
    [Min(0.01f)] [SerializeField] private float _minimumPitch = 0.9f;
    [Min(0.01f)] [SerializeField] private float _maximumPitch = 1.1f;

    private void OnEnable()
    {
        _motionChannels.EntranceStarted += PlayEntranceSound;
    }

    private void OnDisable()
    {
        _motionChannels.EntranceStarted -= PlayEntranceSound;
    }

    private void PlayEntranceSound()
    {
        _audioSource.pitch = Random.Range(_minimumPitch, _maximumPitch);
        _audioSource.PlayOneShot(_clip);
    }
}
