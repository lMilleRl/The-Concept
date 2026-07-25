public readonly struct MovementStateData
{
    public readonly IToggleable[] StateBehaviours;
    public readonly IMovementStateListener[] MovementStateListeners;
    public readonly IStepEffectsProfileController StepEffectsProfileController;
    public readonly StepEffectsProfileType StepEffectsProfileType;

    public MovementStateData(
        IToggleable[] stateBehaviours,
        IMovementStateListener[] movementStateListeners,
        IStepEffectsProfileController stepEffectsProfileController,
        StepEffectsProfileType stepEffectsProfileType)
    {
        StateBehaviours = stateBehaviours;
        MovementStateListeners = movementStateListeners;
        StepEffectsProfileController = stepEffectsProfileController;
        StepEffectsProfileType = stepEffectsProfileType;
    }
}
