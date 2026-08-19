public struct PlayerStateMachineDependencies
{
    public IMoveInput PlayerMoveInput;
    public IMoveInput PlayerClimbingInput;
    public IMoveInput CutsceneCommandInput;
    public IPlayerMovement Movement;
    public IInteractionPlayerInput InteractionInput;
    public IPlayerInteraction Interaction;
    public ClimbingPlayerStateData ClimbingStateData;
    public IToggleable[] GameplayStateAttachedBehaviours;
    public IMovementStateListener[] MovementStateListeners;
    public IStepEffectsProfileController StepEffectsProfileController;
    public IPlayerMovementStateReceiver MovementStateReceiver;

    public PlayerStateMachineDependencies(
        IMoveInput playerMoveInput,
        IMoveInput playerClimbingInput,
        IMoveInput cutsceneCommandInput,
        IPlayerMovement movement,
        IInteractionPlayerInput interactionInput,
        IPlayerInteraction interaction,
        ClimbingPlayerStateData climbingStateData,
        IToggleable[] gameplayStateAttachedBehaviours,
        IMovementStateListener[] movementStateListeners,
        IStepEffectsProfileController stepEffectsProfileController,
        IPlayerMovementStateReceiver movementStateReceiver)
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
        MovementStateReceiver = movementStateReceiver;
    }
}