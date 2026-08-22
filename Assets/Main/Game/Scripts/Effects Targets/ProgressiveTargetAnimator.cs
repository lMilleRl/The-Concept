using System.Collections;
using System.Collections.Generic;
using TextBox;
using UnityEngine;

public class ProgressiveTargetAnimator : MonoBehaviour
{
    [SerializeField] private ProgressiveTargetBase[] _targets;
    [SerializeField, Min(0f)] private float _fadeInDuration = 1f;
    [SerializeField, Min(0f)] private float _holdDuration = 2f;
    [SerializeField, Min(0f)] private float _fadeOutDuration = 1f;
    [SerializeField] private EaseType _ease = EaseType.Linear;

    private Coroutine _routine;

    public void Play()
    {
        Stop();
        _routine = StartCoroutine(Animate());
    }

    public void Stop()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        SetAllProgress(0f);
    }

    private IEnumerator Animate()
    {
        if (_targets == null || _targets.Length == 0)
            yield break;

        SetAllProgress(0f);

        if (_fadeInDuration > 0f)
            yield return Fade(0f, 1f, _fadeInDuration);

        SetAllProgress(1f);

        if (_holdDuration > 0f)
            yield return new WaitForSeconds(_holdDuration);

        if (_fadeOutDuration > 0f)
            yield return Fade(1f, 0f, _fadeOutDuration);

        SetAllProgress(0f);
        _routine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = ApplyEase(t, _ease);
            SetAllProgress(Mathf.LerpUnclamped(from, to, eased));
            yield return null;
        }

        SetAllProgress(to);
    }

    private void SetAllProgress(float progress)
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i] != null)
                _targets[i].SetProgress(progress);
        }
    }

    private static float ApplyEase(float t, EaseType ease)
    {
        return ease switch
        {
            EaseType.None => 1f,
            EaseType.Linear => t,
            EaseType.EaseInQuad => t * t,
            _ => t
        };
    }

    private void OnDestroy()
    {
        Stop();
    }
}
