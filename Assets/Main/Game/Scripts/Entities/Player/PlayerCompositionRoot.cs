using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerInputActiveHandler))]
public class PlayerCompositionRoot : MonoBehaviour
{
    [SerializeField] private PlayerMovementInput _movementInput;
    [SerializeField] private PlayerInteractionInput _interactionInput;
    [SerializeField] private PlayerInteractionActivator _interactionActivator;

    private PlayerMovement _movement;
    private PlayerInteraction _interaction;
    private PlayerInputActiveHandler _inputActiveHandler;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _interaction = GetComponent<PlayerInteraction>();
        _inputActiveHandler = GetComponent<PlayerInputActiveHandler>();

        _interactionActivator = GetComponentInChildren<PlayerInteractionActivator>();

        _movement.Init(_movementInput);
        _interaction.Init(_interactionInput);
        _interactionActivator.Init(_movementInput);
        _inputActiveHandler.Init(new IPlayerInput[] { _movementInput, _interactionInput });
    }
}
