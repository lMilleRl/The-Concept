using UnityEngine;

public class ClimbingPlayerState : MovementState
{
    private ITriggerDetector _ladderDetector;
    private GameObject _playerCollisionsDetector;
    private int _playerCollisionsDetectorInitialLayer;
    private int _ignoreGroundLayer;
    private Vector2 _playerAttachmentEnterPoint;
    private Vector2 _playerAttachmentExitPoint;
    private Transform _playerTransform;
    private SpriteRenderer _playerSpriteRenderer;
    private int _playerInitialOrder;
    private IPlayerMovement _movement;
    private Rigidbody2D _playerRigidBody;
    private Collider2D _playerCollider;
    private IMoveInput _climbingInput;
    private SpriteRenderer _ladderSpriteRenderer;
    private Animator _animator;
    private IPlayerMovementStateReceiver _movementStateReceiver;

    private const float ClimbInputDeadZone = 0.01f;
    private static readonly int NormalizedSpeedParam = Animator.StringToHash("NormalizedSpeed");
    private static readonly int ClimbingStateHash = Animator.StringToHash("Base Layer.Climbing_mikesuit_moving");

    private readonly bool _limitClimbPhaseAdvanceByDuration;
    private readonly float _climbMiniClickMaxDuration;
    private readonly float _climbPhaseSnapTolerance;
    private float _climbInputStartedAt;
    private bool _hadClimbInput;
    private bool _pendingMiniClickPhaseAdvance;

    public ClimbingPlayerState(ClimbingPlayerStateData data) : base(data.MovementStateData)
    {
        _ladderDetector = data.LadderDetector;
        _ladderDetector.Triggered += HandleCollision;
        _playerCollisionsDetector = data.PlayerCollisionsDetector;
        _ignoreGroundLayer = data.IgnoreGroundLayer;
        _playerTransform = data.PlayerTransform;
        _playerSpriteRenderer = data.PlayerSpriteRenderer;
        _movement = data.PlayerMovement;
        _climbingInput = data.ClimbingMoveInput;
        _animator = data.PlayerAnimator;
        _playerRigidBody = data.PlayerRigidBody;
        _playerCollider = data.PlayerCollider;
        _movementStateReceiver = data.MovementStateReceiver;
        _limitClimbPhaseAdvanceByDuration = data.LimitClimbPhaseAdvanceByDuration;
        _climbMiniClickMaxDuration = Mathf.Max(0f, data.ClimbMiniClickMaxDuration);
        _climbPhaseSnapTolerance = Mathf.Clamp(data.ClimbPhaseSnapTolerance, 0f, 0.5f);
    }

    public override void Enter()
    {
        _playerTransform.position = _playerAttachmentEnterPoint;
        _movement.SetInput(_climbingInput);
        _playerRigidBody.velocity = Vector2.zero;
        _playerCollider.enabled = false;
        _playerCollisionsDetectorInitialLayer = _playerCollisionsDetector.layer;
        _playerCollisionsDetector.layer = _ignoreGroundLayer;
        _movementStateReceiver.SetMovementState(PlayerMovementStateType.Climbing);
        SetPlayerOrderAboveLadder();

        _climbInputStartedAt = 0f;
        _hadClimbInput = false;
        _pendingMiniClickPhaseAdvance = false;
        EnterMovementEffects();
    }

    public override void Update()
    {
        _animator.SetFloat(NormalizedSpeedParam, _movement.NormalizedSpeed);
        UpdateClimbPhaseOnMiniClick();
        UpdateMovementEffects();
    }

    private void UpdateClimbPhaseOnMiniClick()
    {
        bool hasClimbInput = Mathf.Abs(_climbingInput.GetRawMovementInput().y) > ClimbInputDeadZone;

        if (hasClimbInput)
        {
            if (!_hadClimbInput)
                _climbInputStartedAt = Time.time;

            _pendingMiniClickPhaseAdvance = false;
        }
        else if (_hadClimbInput)
        {
            var inputDuration = Time.time - _climbInputStartedAt;
            _pendingMiniClickPhaseAdvance = !_limitClimbPhaseAdvanceByDuration ||
                                           inputDuration <= _climbMiniClickMaxDuration;
        }

        _hadClimbInput = hasClimbInput;

        if (_pendingMiniClickPhaseAdvance && _movement.NormalizedSpeed <= 0f && TryAdvanceClimbPhase())
            _pendingMiniClickPhaseAdvance = false;
    }

    private bool TryAdvanceClimbPhase()
    {
        if (!_animator.HasState(0, ClimbingStateHash))
            return true;

        if (_animator.IsInTransition(0))
            return false;

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.fullPathHash != ClimbingStateHash)
            return false;

        var phase = Mathf.Repeat(stateInfo.normalizedTime, 1f);
        if (Mathf.Abs(phase - 0.5f) <= _climbPhaseSnapTolerance)
            return true;

        var cycle = Mathf.Floor(stateInfo.normalizedTime);
        var nextPhase = phase < 0.5f ? 0.5f : 1f;
        _animator.Play(stateInfo.fullPathHash, 0, cycle + nextPhase);
        return true;
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        _playerTransform.position = _playerAttachmentExitPoint;
        _playerRigidBody.velocity = Vector2.zero;
        _playerCollider.enabled = true;
        _playerCollisionsDetector.layer = _playerCollisionsDetectorInitialLayer;
        _movement.SetInput(null);
        _movementStateReceiver.SetMovementState(PlayerMovementStateType.Idle);
        RestorePlayerOrder();
        
        ExitMovementEffects();
    }

    private void HandleCollision(Collider2D other)
    {
        if (other.TryGetComponent(out LadderTrigger ladderTrigger))
        {
            _playerAttachmentEnterPoint = ladderTrigger.AttachmentPlayerEnterPoint.position;
            _playerAttachmentExitPoint = ladderTrigger.AttachmentPlayerExitPoint.position;
            _ladderSpriteRenderer = ladderTrigger.LadderSpriteRenderer;
        }
    }

    private void SetPlayerOrderAboveLadder()
    {
        if (_playerSpriteRenderer == null)
            return;

        _playerInitialOrder = _playerSpriteRenderer.sortingOrder;

        if (_ladderSpriteRenderer != null)
            _playerSpriteRenderer.sortingOrder = _ladderSpriteRenderer.sortingOrder + 1;
        else
            _playerSpriteRenderer.sortingOrder = _playerInitialOrder + 1;
    }

    private void RestorePlayerOrder()
    {
        if (_playerSpriteRenderer == null)
            return;

        _playerSpriteRenderer.sortingOrder = _playerInitialOrder;
    }

    protected override bool IsMoving()
    {
        return _climbingInput != null && _movement.IsMovingByInput;
    }
    
    ~ClimbingPlayerState()
    {
        _ladderDetector.Triggered -= HandleCollision;
    }
}