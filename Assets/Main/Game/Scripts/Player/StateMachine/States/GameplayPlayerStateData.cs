public struct GameplayPlayerStateData
{
    public PlayerMovement PlayerMovement;
    public PlayerInteraction PlayerInteraction;
    public IInteractionPlayerInput InteractionInput;
    public IMoveInput MoveInput;

    public GameplayPlayerStateData(
        PlayerMovement playerMovement,
        PlayerInteraction playerInteraction,
        IInteractionPlayerInput interactionInput,
        IMoveInput moveInput)
    {
        PlayerMovement = playerMovement;
        PlayerInteraction = playerInteraction;
        InteractionInput = interactionInput;
        MoveInput = moveInput;
    }
}
