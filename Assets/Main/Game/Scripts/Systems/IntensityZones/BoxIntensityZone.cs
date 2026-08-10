using System;
using UnityEngine;

public class BoxIntensityZone : MonoBehaviour
{
    [SerializeField] private Transform _listener;
    [SerializeField] private Vector2 _size = Vector2.one;
    [SerializeField] [Min(0f)] private float _falloffDistance = 3f;
    [SerializeField] [Min(0f)] private float _smoothSpeed = 4f;
    [SerializeField] private bool _invertIntensity;
    [SerializeField] private SmoothValueEffect[] _effects;

    private const float ZeroThreshold = 0.0001f;

    private float _currentIntensity;
    private bool _wasInside;

    private void Awake()
    {
        if (_listener == null)
            return;

        _currentIntensity = GetIntensity(_listener.position);

        if (_invertIntensity)
            _currentIntensity = 1f - _currentIntensity;
    }

    private void Update()
    {
        if (_listener == null)
            return;

        float rawIntensity = GetIntensity(_listener.position);
        float target = _invertIntensity ? 1f - rawIntensity : rawIntensity;
        bool isInside = target > 0f;

        if (isInside && !_wasInside)
        {
            foreach (var effect in _effects)
            {
                if (effect != null)
                    effect.Initialize();
            }
        }

        _currentIntensity = Smooth(_currentIntensity, target, _smoothSpeed);

        foreach (var effect in _effects)
        {
            if (effect != null)
                effect.Apply(_currentIntensity);
        }

        _wasInside = isInside;
    }

    private float GetIntensity(Vector2 listenerPosition)
    {
        Vector2 center = transform.position;
        Vector2 halfSize = _size * 0.5f;
        Vector2 closestPoint = new Vector2(
            Mathf.Clamp(listenerPosition.x, center.x - halfSize.x, center.x + halfSize.x),
            Mathf.Clamp(listenerPosition.y, center.y - halfSize.y, center.y + halfSize.y));

        float distance = Vector2.Distance(listenerPosition, closestPoint);
        if (_falloffDistance <= 0f)
            return distance <= 0f ? 1f : 0f;

        return Mathf.Clamp01(1f - distance / _falloffDistance);
    }

    private float Smooth(float current, float target, float speed)
    {
        if (speed <= 0f)
            return target;

        float interpolation = 1f - Mathf.Exp(-speed * Time.deltaTime);
        float result = Mathf.Lerp(current, target, interpolation);

        if (Mathf.Abs(result - target) < ZeroThreshold)
            return target;

        return result;
    }

    private void OnDisable()
    {
        if (_wasInside)
        {
            foreach (var effect in _effects)
            {
                if (effect != null)
                    effect.OnDisabled(_smoothSpeed);
            }
        }

        _wasInside = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, _size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireCube(transform.position, _size + Vector2.one * (_falloffDistance * 2f));
    }
}
