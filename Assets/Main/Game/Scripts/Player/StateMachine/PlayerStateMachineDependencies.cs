public struct PlayerStateMachineDependencies
{
    public IMoveInput PlayerMoveInput;
    public IMoveInput PlayerClimbingInput;
    public IMoveInput CutsceneCommandInput;
    public PlayerMovement Movement;
    public IInteractionPlayerInput InteractionInput;
    public PlayerInteraction Interaction;
    public ClimbingPlayerStateData ClimbingStateData;
    public IToggleable[] GameplayStateAttachedBehaviours;
    public IMovementStateListener[] MovementStateListeners;
    public IStepEffectsProfileController StepEffectsProfileController;

    public PlayerStateMachineDependencies(
        IMoveInput playerMoveInput,
        IMoveInput playerClimbingInput,
        IMoveInput cutsceneCommandInput,
        PlayerMovement movement,
        IInteractionPlayerInput interactionInput,
        PlayerInteraction interaction,
        ClimbingPlayerStateData climbingStateData,
        IToggleable[] gameplayStateAttachedBehaviours,
        IMovementStateListener[] movementStateListeners,
        IStepEffectsProfileController stepEffectsProfileController)
    {
        PlayerMoveInput = playerMoveInput;
        PlayerClimbingInput = playerClimbingInput;
        CutsceneCommandInput = cutsceneCommandInput;
        Movement = movement;
        InteractionInput = interactionInput;
        Interaction = interaction;
        ClimbingStateData = climbingStateData;
        GameplayStateAttachedBehaviours = gameplayStateAttachedBehaviours;
        MovementStateListeners = movementStateListeners;
        StepEffectsProfileController = stepEffectsProfileController;
    }
}