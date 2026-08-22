using TextBox;
using UnityEngine;

public class ProgressiveTargetAudioListener : MonoBehaviour
{
    [SerializeField] private ProgressiveTargetBase _target;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AnimationCurve _volumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve _pitchCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [SerializeField] private bool _stopAtZero = true;

    private void OnEnable()
    {
        if (_target != null)
            _target.ProgressChanged += HandleProgress;
    }

    private void OnDisable()
    {
        if (_target != null)
            _target.ProgressChanged -= HandleProgress;
    }

    private void HandleProgress(float progress)
    {
        if (_audioSource == null)
            return;

        float volume = _volumeCurve.Evaluate(progress);
        float pitch = _pitchCurve.Evaluate(progress);

        if (progress > 0f)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();

            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
        }
        else
        {
            if (_stopAtZero && _audioSource.isPlaying)
                _audioSource.Stop();
            else
                _audioSource.volume = 0f;
        }
    }
}
