using System;

public interface IButtonActivationZone
{
    event Action PointerEntered;
    event Action PointerExited;
    event Action Clicked;

    void SetActivationEnabled(bool enabled);
}
