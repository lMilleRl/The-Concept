using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PressAnyButtonHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private Ease _easeAppearing;
    [Min(0f)] [SerializeField] private float _beforeAppearingDuration;
    [Min(0f)] [SerializeField] private float _appearingDuration;
    [Range(0f, 1f)] [SerializeField] private float _targetAlpha = 1f;
    [Min(0f)] [SerializeField] private float _beforeActivationDuration;

    [SerializeField] private UnityEvent _anyButtonPressed;
    private bool _isButtonCheckingActive;
    
    private void Start()
    {
        _fadeGroup.alpha = 0f;
        Invoke(nameof(StartAppearing), _beforeAppearingDuration);
        Invoke(nameof(SetCheckingButtonActive), _beforeActivationDuration);
    }

    private void Update()
    {
        if (_isButtonCheckingActive && Input.anyKeyDown)
        {
            _anyButtonPressed?.Invoke();
        }
    }

    private void StartAppearing()
    {
        _fadeGroup.DOFade(_targetAlpha, _appearingDuration).SetEase(_easeAppearing);
    }
    
    private void SetCheckingButtonActive()
    {
        _isButtonCheckingActive = true;
    }
}
