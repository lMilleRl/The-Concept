using UnityEngine;

public abstract class InteractionBase : MonoBehaviour, IPlayerInteraction
{
    public abstract bool CanInteract { get; }
    public abstract void SetInput(IInteractionPlayerInput input);
    public abstract void LaunchDelay();
    public abstract void OnInteractionTriggerEnter(Collider2D other);
    public abstract void OnInteractionTriggerExit(Collider2D other);
}
