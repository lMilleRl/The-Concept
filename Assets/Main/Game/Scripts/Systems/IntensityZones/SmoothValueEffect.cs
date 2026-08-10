using System.Collections;
using UnityEngine;

public abstract class SmoothValueEffect : MonoBehaviour, IIntensityEffect
{
    protected const float ZeroApproximation = 0.0001f;
    
    protected bool _wasZeroIntensity;
    
    private float _minSnapshot;
    private float _maxSnapshot;
    private float _currentValue;
    private bool _initialized;
    private Coroutine _releaseCoroutine;

    public void Initialize()
    {
        StopRelease();

        _minSnapshot = GetMinValue();
        _maxSnapshot = GetMaxValue();
        _currentValue = GetCurrentValue();
        _initialized = true;
    }

    public void Apply(float intensity)
    {
        if (!_initialized)
            Initialize();

        StopRelease();

        float clampedIntensity = Mathf.Clamp01(intensity);
        _currentValue = Mathf.Lerp(_minSnapshot, _maxSnapshot, clampedIntensity);

        if (ShouldSetValue(clampedIntensity))
        {
            SetValue(_currentValue);
            _wasZeroIntensity = clampedIntensity >= ZeroApproximation;
        }
    }

    protected virtual bool ShouldSetValue(float intensity) => intensity > ZeroApproximation || !_wasZeroIntensity;

    public void OnDisabled(float releaseSpeed)
    {
        if (!_initialized)
            return;

        StopRelease();

        if (!gameObject.activeInHierarchy)
            return;

        _releaseCoroutine = StartCoroutine(ReleaseCoroutine(releaseSpeed));
    }

    private IEnumerator ReleaseCoroutine(float releaseSpeed)
    {
        if (releaseSpeed <= 0f)
        {
            _currentValue = _minSnapshot;
            SetValue(_currentValue);
            _releaseCoroutine = null;
            yield break;
        }

        while (Mathf.Abs(_currentValue - _minSnapshot) > 0.001f)
        {
            float interpolation = 1f - Mathf.Exp(-releaseSpeed * Time.deltaTime);
            _currentValue = Mathf.Lerp(_currentValue, _minSnapshot, interpolation);
            SetValue(_currentValue);
            yield return null;
        }

        _currentValue = _minSnapshot;
        SetValue(_currentValue);
        _releaseCoroutine = null;
    }

    private void StopRelease()
    {
        if (_releaseCoroutine == null)
            return;

        StopCoroutine(_releaseCoroutine);
        _releaseCoroutine = null;
    }

    protected abstract float GetMinValue();
    protected abstract float GetMaxValue();
    protected abstract float GetCurrentValue();
    protected abstract void SetValue(float value);
}
