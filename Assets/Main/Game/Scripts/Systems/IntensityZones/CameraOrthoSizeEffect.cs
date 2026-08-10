using Cinemachine;
using UnityEngine;

public class CameraOrthoSizeEffect : SmoothValueEffect
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _targetSize;

    protected override float GetMinValue()
    {
        return _virtualCamera != null ? _virtualCamera.m_Lens.OrthographicSize : 0f;
    }

    protected override float GetMaxValue() => _targetSize;

    protected override float GetCurrentValue()
    {
        return _virtualCamera != null ? _virtualCamera.m_Lens.OrthographicSize : 0f;
    }


    protected override void SetValue(float value)
    {
        if (_virtualCamera != null)
        {
            _virtualCamera.m_Lens.OrthographicSize = value;
            
        }
    }
}
