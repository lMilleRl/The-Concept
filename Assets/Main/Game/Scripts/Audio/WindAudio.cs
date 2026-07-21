using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindAudio : MonoBehaviour, IWindSignal
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] [Min(64)] private int _sampleCount = 128;
    [SerializeField] [Min(0.0001f)] private float _maximumRms = 0.1f;
    [SerializeField] [Min(0f)] private float _attackSpeed = 8f;
    [SerializeField] [Min(0f)] private float _releaseSpeed = 2f;

    public float NormalizedStrength { get; private set; }

    private float[] _samples;

    private void Awake()
    {
        _audioSource ??= GetComponent<AudioSource>();
        _sampleCount = Mathf.NextPowerOfTwo(_sampleCount);
        _samples = new float[_sampleCount];
    }

    private void Update()
    {
        float targetStrength = GetNormalizedRms();
        float smoothingSpeed = targetStrength > NormalizedStrength ? _attackSpeed : _releaseSpeed;
        NormalizedStrength = Smooth(NormalizedStrength, targetStrength, smoothingSpeed);
    }

    private float GetNormalizedRms()
    {
        if (!_audioSource.isPlaying) return 0f;

        _audioSource.GetOutputData(_samples, 0);

        float sumOfSquares = 0f;
        foreach (float sample in _samples)
            sumOfSquares += sample * sample;

        float rms = Mathf.Sqrt(sumOfSquares / _samples.Length);
        return Mathf.Clamp01(rms / _maximumRms);
    }

    private float Smooth(float current, float target, float speed)
    {
        float interpolation = 1f - Mathf.Exp(-speed * Time.deltaTime);
        return Mathf.Lerp(current, target, interpolation);
    }
}
