using System.Collections;
using UnityEngine;

public class CutsceneCommandInput : MonoBehaviour, IMoveInput, IMovementSpeedSource
{
    private const float DefaultSpeedMultiplier = 1f;

    public bool IsInputEnabled { get; set; }
    public float SpeedMultiplier { get; private set; } = DefaultSpeedMultiplier;

    private Vector2 _currentInput;
    private Coroutine _currentResetCoroutine;

    public void Execute(CutsceneMovementCommand command)
    {
        if (command == null)
            return;

        SetMove(command.Direction, command.DurationInSec, command.SpeedMultiplier);
    }

    public void SetMove(Vector2 direction, float movementTimeInSec, float speedMultiplier)
    {
        direction.Normalize();
        _currentInput = direction;
        SpeedMultiplier = Mathf.Clamp01(speedMultiplier);

        if (_currentResetCoroutine != null)
            StopCoroutine(_currentResetCoroutine);
        _currentResetCoroutine = StartCoroutine(WaitForReset(movementTimeInSec));
    }

    public Vector2 GetMovementInput()
    {
        return _currentInput;
    }

    public Vector2 GetRawMovementInput()
    {
        return new Vector2(GetRawCoordinate(_currentInput.x), GetRawCoordinate(_currentInput.y));
    }

    private IEnumerator WaitForReset(float timeInSec)
    {
        yield return new WaitForSeconds(timeInSec);
        Reset();
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
        if (_currentResetCoroutine != null)
        {
            StopCoroutine(_currentResetCoroutine);
            _currentResetCoroutine = null;
        }

        Reset();
    }

    private void Reset()
    {
        _currentInput = Vector2.zero;
        SpeedMultiplier = DefaultSpeedMultiplier;
    }
}