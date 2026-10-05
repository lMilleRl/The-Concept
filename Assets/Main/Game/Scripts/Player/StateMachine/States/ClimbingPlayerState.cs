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

    private static readonly int ClimbProgressParam = Animator.StringToHash("ClimbProgress");
    private const float ClimbInputDeadZone = 0.01f;

    private float _climbUnitsPerCycle;
    private float _climbPhase;
    private Vector2 _lastClimbPosition;
    private bool _hadClimbInput;

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
        _climbUnitsPerCycle = Mathf.Max(0.01f, data.ClimbUnitsPerCycle);
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

        _climbPhase = 0f;
        _lastClimbPosition = _playerTransform.position;
        _hadClimbInput = false;
        _animator.SetFloat(ClimbProgressParam, 0f);
        
        EnterMovementEffects();
    }

    public override void Update()
    {
        var position = (Vector2)_playerTransform.position;
        _climbPhase += (position - _lastClimbPosition).magnitude / _climbUnitsPerCycle;
        _lastClimbPosition = position;

        // при остановке переключаем хват на противоположный (позиции 0 и 0.5 цикла)
        var hasClimbInput = Mathf.Abs(_climbingInput.GetMovementInput().y) > ClimbInputDeadZone;
        if (!hasClimbInput && _hadClimbInput)
            _climbPhase += 0.5f;
        _hadClimbInput = hasClimbInput;

        _animator.SetFloat(ClimbProgressParam, Mathf.Repeat(_climbPhase, 1f));
        UpdateMovementEffects();
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