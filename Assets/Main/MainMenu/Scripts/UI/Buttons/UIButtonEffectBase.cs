using UnityEngine;

public abstract class UIButtonEffectBase : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _activationZoneComponent;

    private IButtonActivationZone _activationZone;
    private bool _isSubscribed;

    protected virtual void Awake()
    {
        _activationZone = _activationZoneComponent as IButtonActivationZone;

        if (_activationZone == null)
        {
            Debug.LogError(
                $"{nameof(UIButtonEffectBase)} requires a component implementing {nameof(IButtonActivationZone)}.",
                this);
        }
    }

    protected virtual void OnEnable()
    {
        if (_activationZone == null)
        {
            return;
        }

        _activationZone.PointerEntered += HandlePointerEntered;
        _activationZone.PointerExited += HandlePointerExited;
        _activationZone.Clicked += HandleClicked;
        _isSubscribed = true;
    }

    protected virtual void OnDisable()
    {
        if (_isSubscribed)
        {
            _activationZone.PointerEntered -= HandlePointerEntered;
            _activationZone.PointerExited -= HandlePointerExited;
            _activationZone.Clicked -= HandleClicked;
            _isSubscribed = false;
        }

        OnEffectDisabled();
    }

    protected abstract void OnActivationZoneEntered();
    protected abstract void OnActivationZoneExited();
    protected abstract void OnActivationZoneClicked();

    protected virtual void OnEffectDisabled()
    {
    }

    private void HandlePointerEntered()
    {
        OnActivationZoneEntered();
    }

    private void HandlePointerExited()
    {
        OnActivationZoneExited();
    }

    private void HandleClicked()
    {
        OnActivationZoneClicked();
    }
}

public abstract class UIButtonHoverEffectBase : UIButtonEffectBase
{
    protected sealed override void OnActivationZoneClicked()
    {
    }
}

public abstract class UIButtonClickEffectBase : UIButtonEffectBase
{
    protected sealed override void OnActivationZoneEntered()
    {
    }

    protected sealed override void OnActivationZoneExited()
    {
    }
}
