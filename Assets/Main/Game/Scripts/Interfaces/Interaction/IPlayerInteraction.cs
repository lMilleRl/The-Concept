public interface IPlayerInteraction
{
    bool CanInteract { get; }
    void SetInput(IInteractionPlayerInput input);
    void LaunchDelay();
}
