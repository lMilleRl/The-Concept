using UnityEngine;

public class PlayerInteractionInput : MonoBehaviour, IInteractionPlayerInput
{
    [SerializeField] private KeyCode _keyToInteract;

    public bool IsInputEnabled { get; set; } = true;

    public bool IsInteractionButtonPressed()
    {
        if (!IsInputEnabled) return false;
        return Input.GetKeyDown(_keyToInteract);
    }
}
