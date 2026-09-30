using System;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonEntranceSide
{
    Left,
    Right
}

[Serializable]
public struct ButtonEntranceItem
{
    public Button Button;
    public MonoBehaviour ActivationZone;
    public Transform Destination;
    [Min(0f)] public float Delay;
    public ButtonEntranceSide FromSide;
}

public sealed class UIButtonEntranceController : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ButtonEntranceItem[] _buttons;
    [Min(0f)] [SerializeField] private float _duration = 0.65f;
    [Min(0f)] [SerializeField] private float _overshoot = 1.2f;
    [Min(0f)] [SerializeField] private float _edgePadding = 40f;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        RectTransform viewport = _canvas.rootCanvas.transform as RectTransform;

        foreach (ButtonEntranceItem item in _buttons)
        {
            IButtonActivationZone activationZone = (IButtonActivationZone)item.ActivationZone;
            item.Button.interactable = false;
            activationZone.SetActivationEnabled(false);
            item.Button.GetComponent<UIButtonMotionChannels>().PlayEntrance(
                viewport,
                item.Destination.position,
                item.FromSide == ButtonEntranceSide.Right,
                item.Delay,
                _duration,
                _overshoot,
                _edgePadding,
                () =>
                {
                    item.Button.interactable = true;
                    activationZone.SetActivationEnabled(true);
                });
        }
    }
}
