
public struct PlayerStateMachineDependencies
{
    public IMoveInput PlayerMoveInput;
    public IMoveInput PlayerClimbingInput;
    public PlayerMovement Movement;
    public IInteractionPlayerInput InteractionInput;
    public PlayerInteraction Interaction;
    public ClimbingPlayerStateData ClimbingStateData;

    public PlayerStateMachineDependencies(
        IMoveInput playerMoveInput,
        IMoveInput playerClimbingInput,
        PlayerMovement movement,
        IInteractionPlayerInput interactionInput,
        PlayerInteraction interaction,
        ClimbingPlayerStateData climbingStateData)
    {
        PlayerMoveInput = playerMoveInput;
        PlayerClimbingInput = playerClimbingInput;
        Movement = movement;
        InteractionInput = interactionInput;
        Interaction = interaction;
        ClimbingStateData = climbingStateData;
    }
}



