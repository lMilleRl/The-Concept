using System.Collections;
using UnityEngine;

public class CutsceneCommandInput : MonoBehaviour, IMoveInput, IMovementSpeedSource, ICutsceneMovementCommandReceiver
{
    private const float DefaultSpeedMultiplier = 1f;

    public bool IsInputEnabled { get; set; }
    public float SpeedMultiplier { get; private set; } = DefaultSpeedMultiplier;

    private Vector2 _currentInput;
    private Coroutine _currentResetCoroutine;
    private Transform _playerTransform;
    private Transform _targetTransform;
    private float _targetStoppingDistance;
    private bool _isMovingToTarget;

    public void Init(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }

    public void Execute(CutsceneMovementCommand command)
    {
        if (command == null)
            return;

        SetMove(command.Direction, command.DurationInSec, command.SpeedMultiplier);
    }

    public void Execute(CutsceneMoveToTargetCommand command)
    {
        CancelResetCoroutine();
        if (command.Target == null)
        {
            Reset();
            return;
        }

        _currentInput = Vector2.zero;
        _targetTransform = command.Target;
        _targetStoppingDistance = Mathf.Max(0f, command.StoppingDistance);
        _isMovingToTarget = true;
        SpeedMultiplier = Mathf.Clamp01(command.SpeedMultiplier);
    }

    public void SetMove(Vector2 direction, float movementTimeInSec, float speedMultiplier)
    {
        CancelResetCoroutine();
        _targetTransform = null;
        _targetStoppingDistance = 0f;
        _isMovingToTarget = false;
        direction.Normalize();
        _currentInput = direction;
        SpeedMultiplier = Mathf.Clamp01(speedMultiplier);
        _currentResetCoroutine = StartCoroutine(WaitForReset(movementTimeInSec));
    }

    public Vector2 GetMovementInput()
    {
        if (!_isMovingToTarget)
            return _currentInput;

        if (_targetTransform == null)
        {
            Reset();
            return Vector2.zero;
        }

        var playerPosition = _playerTransform != null
            ? (Vector2)_playerTransform.position
            : (Vector2)transform.position;
        var targetDirection = (Vector2)_targetTransform.position - playerPosition;
        if (targetDirection.sqrMagnitude <= _targetStoppingDistance * _targetStoppingDistance)
        {
            Reset();
            return Vector2.zero;
        }

        _currentInput = targetDirection.normalized;
        return _currentInput;
    }

    public Vector2 GetRawMovementInput()
    {
        var movementInput = GetMovementInput();
        return new Vector2(GetRawCoordinate(movementInput.x), GetRawCoordinate(movementInput.y));
    }

    private IEnumerator WaitForReset(float timeInSec)
    {
        yield return new WaitForSeconds(timeInSec);
        Reset();
        _currentResetCoroutine = null;
    }

    private void CancelResetCoroutine()
    {
        if (_currentResetCoroutine == null)
            return;

        StopCoroutine(_currentResetCoroutine);
        _currentResetCoroutine = null;
    }

    private int GetRawCoordinate(float coord)
    {
        if (Mathf.Approximately(coord, 0f))
            return 0;
        if (coord > 0f)
            return 1;

        return -1;
    }

    private void OnDisable()
    {
        CancelResetCoroutine();
        Reset();
    }

    private void Reset()
    {
        _currentInput = Vector2.zero;
        _targetTransform = null;
        _targetStoppingDistance = 0f;
        _isMovingToTarget = false;
        SpeedMultiplier = DefaultSpeedMultiplier;
    }
}