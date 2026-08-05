using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    private const float HalfChance = 0.5f;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _sounds;

    [Header("Pitch")]
    [SerializeField] private bool _randomizePitch = true;
    [SerializeField] [Min(-3f)] private float _minPitch = 0.9f;
    [SerializeField] [Min(-3f)] private float _maxPitch = 1.1f;

    [Header("Volume")]
    [SerializeField] [Range(0f, 1f)] private float _volume = 1f;

    private int _previousIndex;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_sounds != null && _sounds.Length > 0)
            _previousIndex = Random.Range(0, _sounds.Length);
    }

    public void Play()
    {
        if (_audioSource == null || _sounds == null || _sounds.Length == 0)
            return;

        int index = GetNextIndex();
        AudioClip clip = _sounds[index];
        if (clip == null)
            return;

        _previousIndex = index;

        _audioSource.pitch = _randomizePitch
            ? Random.Range(_minPitch, _maxPitch)
            : 1f;

        _audioSource.PlayOneShot(clip, _volume);
    }

    private int GetNextIndex()
    {
        if (_sounds.Length <= 1)
            return 0;

        bool canPickLower = _previousIndex > 0;
        bool canPickUpper = _previousIndex < _sounds.Length - 1;

        if (canPickLower && canPickUpper)
        {
            return Random.value < HalfChance
                ? Random.Range(0, _previousIndex)
                : Random.Range(_previousIndex + 1, _sounds.Length);
        }

        if (canPickLower)
            return Random.Range(0, _previousIndex);

        return Random.Range(_previousIndex + 1, _sounds.Length);
    }
}
