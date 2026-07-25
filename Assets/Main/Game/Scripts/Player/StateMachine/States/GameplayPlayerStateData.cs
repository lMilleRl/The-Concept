public readonly struct GameplayPlayerStateData
{
    public readonly PlayerMovement PlayerMovement;
    public readonly PlayerInteraction PlayerInteraction;
    public readonly IInteractionPlayerInput InteractionInput;
    public readonly IMoveInput MoveInput;
    public readonly MovementStateData MovementStateData;

    public GameplayPlayerStateData(
        PlayerMovement playerMovement,
        PlayerInteraction playerInteraction,
        IInteractionPlayerInput interactionInput,
        IMoveInput moveInput,
        MovementStateData movementStateData)
    {
        PlayerMovement = playerMovement;
        PlayerInteraction = playerInteraction;
        InteractionInput = interactionInput;
        MoveInput = moveInput;
        MovementStateData = movementStateData;
    }
}