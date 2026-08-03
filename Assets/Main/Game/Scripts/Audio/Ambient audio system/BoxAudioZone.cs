using UnityEngine;

public class AmbientAudioZone : MonoBehaviour
{
    [SerializeField] private Transform _listener;
    [SerializeField] private AudioSource[] _audioSources;
    [SerializeField] private Vector2 _size = Vector2.one;
    [SerializeField] [Min(0f)] private float _falloffDistance = 3f;
    [SerializeField] [Min(0f)] private float _volumeSmoothSpeed = 4f;

    private float[] _maxVolumes;
    private float _currentIntensity;

    private void Awake()
    {
        _maxVolumes = new float[_audioSources.Length];
        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (_audioSources[i] == null) continue;

            _maxVolumes[i] = _audioSources[i].volume;
            _audioSources[i].loop = true;
            _audioSources[i].volume = 0f;
            _audioSources[i].Play();
        }
    }

    private void Update()
    {
        if (_listener == null) return;

        float targetIntensity = GetIntensity(_listener.position);
        _currentIntensity = Smooth(_currentIntensity, targetIntensity, _volumeSmoothSpeed);

        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (_audioSources[i] == null) continue;
            _audioSources[i].volume = _maxVolumes[i] * _currentIntensity;
        }
    }

    private float GetIntensity(Vector2 listenerPosition)
    {
        Vector2 center = transform.position;
        Vector2 halfSize = _size * 0.5f;
        var closestPoint = new Vector2(
            Mathf.Clamp(listenerPosition.x, center.x - halfSize.x, center.x + halfSize.x),
            Mathf.Clamp(listenerPosition.y, center.y - halfSize.y, center.y + halfSize.y));

        float distance = Vector2.Distance(listenerPosition, closestPoint);
        if (_falloffDistance <= 0f) return distance <= 0f ? 1f : 0f;

        return Mathf.Clamp01(1f - distance / _falloffDistance);
    }

    private float Smooth(float current, float target, float speed)
    {
        if (speed <= 0f) return target;

        float interpolation = 1f - Mathf.Exp(-speed * Time.deltaTime);
        return Mathf.Lerp(current, target, interpolation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, _size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireCube(transform.position, _size + Vector2.one * (_falloffDistance * 2f));
    }
}
