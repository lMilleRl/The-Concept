using UnityEngine;

public class TriggerAudio : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip _clip;
    [Range(0f, 1f)] [SerializeField] private float _volume = 1f;
    [SerializeField] private bool _playOnInteract = true;

    public void Activate()
    {
        if (_playOnInteract)
            Play();
    }

    public void Play()
    {
        if (_clip == null) return;
        
        AudioSource.PlayClipAtPoint(_clip, transform.position, _volume);
    }

    public void Play(AudioClip overrideClip)
    {
        if (overrideClip == null) return;
        
        AudioSource.PlayClipAtPoint(overrideClip, transform.position, _volume);
    }
}
