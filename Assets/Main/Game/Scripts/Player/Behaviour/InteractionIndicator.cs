using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    private static readonly int IsShowingParam = Animator.StringToHash("IsShowing");
    private static readonly int IsBlinkingParam = Animator.StringToHash("IsBlinking");

    [SerializeField] private SpriteRenderer _indicatorSprite;
    [SerializeField] private InteractionBase _interaction;
    [SerializeField] private Animator _animator;

    [Header("Blinking")]
    [SerializeField] private float _blinkInterval = 2f;
    [SerializeField] private float _blinkRandomSpread = 0.5f;

    private bool _wasVisible;
    private float _nextBlinkTime;

    private void Awake()
    {
        if (_indicatorSprite != null)
            _indicatorSprite.enabled = false;
    }

    private void Update()
    {
        if (_interaction == null || _indicatorSprite == null)
            return;

        bool isVisible = _interaction.CanInteract;
        _indicatorSprite.enabled = isVisible;

        HandleVisibilityChanged(isVisible);
        HandleBlinking(isVisible);

        _wasVisible = isVisible;
    }

    private void HandleVisibilityChanged(bool isVisible)
    {
        if (isVisible && !_wasVisible)
        {
            if (_animator != null)
                _animator.SetTrigger(IsShowingParam);
            ScheduleNextBlink();
        }
    }

    private void HandleBlinking(bool isVisible)
    {
        if (isVisible && Time.time >= _nextBlinkTime)
        {
            if (_animator != null)
                _animator.SetTrigger(IsBlinkingParam);
            ScheduleNextBlink();
        }
    }

    private void ScheduleNextBlink()
    {
        float spread = _blinkRandomSpread > 0f ? Random.Range(-_blinkRandomSpread, _blinkRandomSpread) : 0f;
        _nextBlinkTime = Time.time + _blinkInterval + spread;
    }
}
