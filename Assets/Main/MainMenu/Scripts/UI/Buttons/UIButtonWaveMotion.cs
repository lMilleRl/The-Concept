using UnityEngine;

[RequireComponent(typeof(UIButtonMotionChannels))]
public sealed class UIButtonWaveMotion : MonoBehaviour
{
    [SerializeField] private float _amplitude = 2.5f;
    [SerializeField] private float _speed = 1.4f;
    [SerializeField] private float _phase;
    
    private UIButtonMotionChannels _motionChannels;

    private void Awake()
    {
        _motionChannels = GetComponent<UIButtonMotionChannels>();
    }

    private void Update()
    {
        _motionChannels.SetWaveOffset(Mathf.Sin(Time.time * _speed + _phase) * _amplitude);
    }

    private void OnDisable()
    {
        _motionChannels.SetWaveOffset(0f);
    }
}
