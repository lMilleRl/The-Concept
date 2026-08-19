public readonly struct GameplayPlayerStateData
{
    public readonly IPlayerMovement PlayerMovement;
    public readonly IPlayerInteraction PlayerInteraction;
    public readonly IInteractionPlayerInput InteractionInput;
    public readonly IMoveInput MoveInput;
    public readonly MovementStateData MovementStateData;
    public readonly IPlayerMovementStateReceiver MovementStateReceiver;

    public GameplayPlayerStateData(
        IPlayerMovement playerMovement,
        IPlayerInteraction playerInteraction,
        IInteractionPlayerInput interactionInput,
        IMoveInput moveInput,
        MovementStateData movementStateData,
        IPlayerMovementStateReceiver movementStateReceiver)
    {
        PlayerMovement = playerMovement;
        PlayerInteraction = playerInteraction;
        InteractionInput = interactionInput;
        MoveInput = moveInput;
        MovementStateData = movementStateData;
        MovementStateReceiver = movementStateReceiver;
    }
}