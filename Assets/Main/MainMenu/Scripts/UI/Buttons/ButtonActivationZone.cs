using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActivationZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IButtonActivationZone
{
    [SerializeField] private Button _button;

    public event Action PointerEntered;
    public event Action PointerExited;
    public event Action Clicked;

    private bool _isPointerInside;
    private bool _activationEnabled = true;

    private void Awake()
    {
        if (_button == null)
        {
            _button = GetComponentInChildren<Button>(true);
        }

        if (_button == null)
        {
            Debug.LogError($"{nameof(ButtonActivationZone)} requires a Button reference.", this);
        }
    }

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(HandleButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleButtonClicked);
        }

        if (_isPointerInside && _activationEnabled)
        {
            PointerExited?.Invoke();
        }

        _isPointerInside = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
        if (_activationEnabled) PointerEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        if (_activationEnabled) PointerExited?.Invoke();
    }

    public void SetActivationEnabled(bool enabled)
    {
        if (_activationEnabled == enabled) return;

        _activationEnabled = enabled;
        if (!_isPointerInside) return;

        if (_activationEnabled) PointerEntered?.Invoke();
        else PointerExited?.Invoke();
    }

    private void HandleButtonClicked()
    {
        if (_activationEnabled) Clicked?.Invoke();
    }
}
