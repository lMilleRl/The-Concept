using UnityEngine;

public class CutsceneMovementSignalBridge : MonoBehaviour
{
    [Tooltip("GameObject, содержащий компонент-получатель команды движения")]
    [SerializeField] private GameObject _commandReceiverObject;
    [SerializeField] private Transform _targetTransform;
    [SerializeField, Range(0f, 1f)] private float _speedMultiplier = 1f;
    [SerializeField, Min(0f)] private float _stoppingDistance = 0.1f;

    private ICutsceneMovementCommandReceiver _commandReceiver;

    private void Awake()
    {
        if (_commandReceiverObject != null)
            _commandReceiverObject.TryGetComponent<ICutsceneMovementCommandReceiver>(out _commandReceiver);
    }

    public void MovePlayerToTarget()
    {
        if (_commandReceiver == null || _targetTransform == null)
            return;

        var command = new CutsceneMoveToTargetCommand(
            _targetTransform,
            _speedMultiplier,
            _stoppingDistance);
        _commandReceiver.Execute(command);
    }
}
