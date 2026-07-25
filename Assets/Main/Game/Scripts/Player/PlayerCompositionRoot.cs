using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerBootstrapStateMachine))]
public class PlayerCompositionRoot : MonoBehaviour
{
    [Header("Behaviours in inspector")] 
    [SerializeField] private PlayerMovementInput _movementInput;
    [SerializeField] private PlayerInteractionInput _interactionInput;
    [SerializeField] private PlayerInteractionActivator _interactionActivator;
    [SerializeField] private FootStepControllerHandler footStepControllerHandler;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private PlayerBootstrapStateMachine _stateMachineBootstrap;

    [Header("Walking footstep audio strategy")] 
    [SerializeField] private FootStepAudioData _walkingFootstepAudioData;
    [SerializeField] private AudioSource _walkingFootstepSoundsPlayer;
    [SerializeField] private float _walkingDistanceBetweenSteps = 0.5f;
    [SerializeField] private float _climbingDistanceBetweenSteps = 0.5f;

    [Header("Climbing audio strategy")]
    [SerializeField] private AudioSource _climbingSoundsPlayer;
    [SerializeField] private AudioClip[] _climbingAudioClips;

    [Header("Footprint strategy")]
    [SerializeField] private FootprintData _footprintData;
    [SerializeField] private Footprint _footprintPrefab;

    [Header("Climbing State Data")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BodyCollisionsDetector _playerCollisionsDetector;
    [SerializeField] private Collider2D _playerBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private int _ignoreGroundLayer;

    private void Start()
    {
        _movement.Init(_movementInput);
        _interaction.Init(_interactionInput);
        _interactionActivator.Init(_movementInput);
        
        InitBootstrapStateMachine();
    }

    private void InitBootstrapStateMachine()
    {
        footStepControllerHandler.Init(GetStepEffectsProfiles());
        
        var climbingInput = new OnLadderPlayerInput();
        
        var movementStateBehaviours = new IToggleable[] { footStepControllerHandler };
        var movementStateListeners = new IMovementStateListener[] { footStepControllerHandler };
        
        var climbingMovementStateData = new MovementStateData(movementStateBehaviours, movementStateListeners, footStepControllerHandler, StepEffectsProfileType.Climbing);
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
            _playerSpriteRenderer,
            climbingMovementStateData);

        var stateMachineDependencies = new PlayerStateMachineDependencies(
            _movementInput,
            climbingInput,
            _movement,
            _interactionInput,
            _interaction,
            climbingStateData,
            movementStateBehaviours,
            movementStateListeners,
            footStepControllerHandler);

        _stateMachineBootstrap.Init(stateMachineDependencies);
    }

    private StepEffectsProfileData[] GetStepEffectsProfiles()
    {
        var walkingFootstepAudioStrategy = new WalkingFootstepAudioStrategy(
            _walkingFootstepAudioData,
            _walkingFootstepSoundsPlayer);
        var snowFootprintStrategy = new FootprintStrategy(
            _footprintData,
            _footprintPrefab);
        var climbingAudioStrategy = new ClimbingAudioStrategy(
            _climbingSoundsPlayer,
            _climbingAudioClips);

        var walkingProfile = new StepEffectsProfileData(
            StepEffectsProfileType.Walking,
            new IStepEffectStrategy[] { walkingFootstepAudioStrategy, snowFootprintStrategy },
            _walkingDistanceBetweenSteps);
        var climbingProfile = new StepEffectsProfileData(
            StepEffectsProfileType.Climbing,
            new IStepEffectStrategy[] { climbingAudioStrategy },
            _climbingDistanceBetweenSteps);

        return new[] { walkingProfile, climbingProfile };
    }
}