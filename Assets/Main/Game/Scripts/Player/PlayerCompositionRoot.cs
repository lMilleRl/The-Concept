using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerBootstrapStateMachine))]
public class PlayerCompositionRoot : MonoBehaviour
{
    [SerializeField] private PlayerMovementInput _movementInput;
    [SerializeField] private PlayerInteractionInput _interactionInput;
    [SerializeField] private PlayerInteractionActivator _interactionActivator;

    [Header("Climbing State Data")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BodyCollisionsDetector _playerCollisionsDetector;
    [SerializeField] private Collider2D _playerBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private int _ignoreGroundLayer;

    private PlayerMovement _movement;
    private PlayerInteraction _interaction;
    private PlayerBootstrapStateMachine _stateMachineBootstrap;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _interaction = GetComponent<PlayerInteraction>();
        _stateMachineBootstrap = GetComponent<PlayerBootstrapStateMachine>();

        _interactionActivator = GetComponentInChildren<PlayerInteractionActivator>();

        _movement.Init(_movementInput);
        _interaction.Init(_interactionInput);
        _interactionActivator.Init(_movementInput);

        var climbingInput = new OnLadderPlayerInput();
        var climbingStateData = new ClimbingPlayerStateData(
            _playerCollisionsDetector,
            _playerCollisionsDetector.gameObject,
            _ignoreGroundLayer,
            transform,
            _movement,
            climbingInput,
            _animator,
            _rigidbody2D,
            _playerBody,
            _playerSpriteRenderer);
        var stateMachineDependencies = new PlayerStateMachineDependencies(
            _movementInput,
            climbingInput,
            _movement,
            _interactionInput,
            _interaction,
            climbingStateData);

        _stateMachineBootstrap.Init(stateMachineDependencies);
    }
}

