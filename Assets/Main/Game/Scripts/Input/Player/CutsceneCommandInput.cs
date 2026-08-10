using System.Collections;
using UnityEngine;

public class CutsceneCommandInput : MonoBehaviour, IMoveInput
{
    public bool IsInputEnabled { get; set; }

    private Vector2 _currentInput;
    private Coroutine _currentResetCoroutine;

    private readonly Vector2 _defaultInput = Vector2.zero;

    public void Execute(CutsceneMovementCommand command)
    {
        if (command == null)
            return;

        SetMove(command.Direction, command.DurationInSec, command.InputMagnitude);
    }

    public void SetMove(Vector2 direction, float movementTimeInSec, float inputMagnitude)
    {
        direction.Normalize();
        _currentInput = direction * inputMagnitude;
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
        _currentInput.x = GetRawCoordinate(_currentInput.x);
        _currentInput.y = GetRawCoordinate(_currentInput.y);
        return _currentInput;
    }

    private IEnumerator WaitForReset(float timeInSec)
    {
        yield return new WaitForSeconds(timeInSec);
        Reset();
    }

    private int GetRawCoordinate(float coord)
    {
        if (Mathf.Approximately(coord, 0f))
            return 0;
        if (coord > 0f)
            return 1;

        return -1;
    }

    private void Reset()
    {
        _currentInput = _defaultInput;
    }
}