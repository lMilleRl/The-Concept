using System;
using System.Collections;
using UnityEngine;

public class TriggerAudio : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] [Range(0f, float.MaxValue)] private float _defaultDelayInSec;
    [SerializeField] private bool _playOnInteract = true;

    private WaitForSeconds _waitForDelay;

    private void Awake()
    {
        _waitForDelay = new WaitForSeconds(_defaultDelayInSec);
    }

    public void Activate()
    {
        if (_playOnInteract)
        {
            StartCoroutine(PlaySequence());
        }
    }

    private IEnumerator PlaySequence()
    {
        foreach (var clip in _clips)
        {
            Play(clip);
            yield return _waitForDelay;
        }
    }
    
    public void Play(AudioClip clip)
    {
        if (clip == null || _audioSource == null) return;
        _audioSource.PlayOneShot(clip);
    }
}
