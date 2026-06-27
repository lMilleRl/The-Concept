using Cinemachine;
using UnityEngine;

public class CameraZoneSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _zoneCamera;
    [SerializeField] private int _activePriority = 20;
    [SerializeField] private int _inactivePriority = 0;

    public void Activate()
    {
        _zoneCamera.Priority = _activePriority;
    }

    public void Deactivate()
    {
        _zoneCamera.Priority = _inactivePriority;
    }
}
