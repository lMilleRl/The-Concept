public abstract class MovementState : State
{
    private readonly IToggleable[] _stateBehaviours;
    private readonly IMovementStateListener[] _movementStateListeners;
    private readonly IStepEffectsProfileController _stepEffectsProfileController;
    private readonly StepEffectsProfileType _stepEffectsProfileType;
    private bool _wasMoving;

    protected MovementState(MovementStateData data)
    {
        _stateBehaviours = data.StateBehaviours;
        _movementStateListeners = data.MovementStateListeners;
        _stepEffectsProfileController = data.StepEffectsProfileController;
        _stepEffectsProfileType = data.StepEffectsProfileType;
    }

    protected void EnterMovementEffects()
    {
        _stepEffectsProfileController.SetProfile(_stepEffectsProfileType);

        foreach (var behaviour in _stateBehaviours)
            behaviour.SetEnabled(true);

        _wasMoving = IsMoving();
        NotifyMovementState(_wasMoving);
    }

    protected void UpdateMovementEffects()
    {
        bool isMoving = IsMoving();
        if (isMoving == _wasMoving)
            return;

        _wasMoving = isMoving;
        NotifyMovementState(isMoving);
    }

    protected void ExitMovementEffects()
    {
        NotifyMovementState(false);

        foreach (var behaviour in _stateBehaviours)
            behaviour.SetEnabled(false);
    }

    protected abstract bool IsMoving();

    private void NotifyMovementState(bool isMoving)
    {
        foreach (var listener in _movementStateListeners)
            listener.SetMovementActive(isMoving);
    }
}