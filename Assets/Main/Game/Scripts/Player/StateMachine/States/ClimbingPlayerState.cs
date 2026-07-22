using UnityEngine;

public class ClimbingPlayerState : State
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
    private PlayerMovement _movement;
    private Rigidbody2D _playerRigidBody;
    private Collider2D _playerCollider;
    private IMoveInput _climbingInput;

    private SpriteRenderer _ladderSpriteRenderer;

    private Animator _animator;

    public ClimbingPlayerState(ClimbingPlayerStateData data)
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
    }

    public override void Enter()
    {
        _playerTransform.position = _playerAttachmentEnterPoint;
        _movement.SetInput(_climbingInput);
        _playerRigidBody.velocity = Vector2.zero;
        _playerCollider.enabled = false;
        _playerCollisionsDetectorInitialLayer = _playerCollisionsDetector.layer;
        _playerCollisionsDetector.layer = _ignoreGroundLayer;
        _animator.SetBool("IsClimbing", true);

        SetPlayerOrderAboveLadder();
    }

    public override void Update()
    {
        _animator.SetFloat("ClimbSpeed", _climbingInput.GetMovementInput().y);
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
        _animator.SetBool("IsClimbing", false);

        RestorePlayerOrder();
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

    ~ClimbingPlayerState()
    {
        _ladderDetector.Triggered -= HandleCollision;
    }
}