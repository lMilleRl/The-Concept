using UnityEngine;

public class ClimbingPlayerState : State  
{
    private ITriggerDetector _ladderDetector;
    private Vector2 _playerAttachmentEnterPoint;
    private Vector2 _playerAttachmentExitPoint;
    private Transform _playerTransform;

    private PlayerMovement _movement;
    private Rigidbody2D _playerRigidBody;
    private Collider2D _playerCollider;
    private IMoveInput _climbingInput;

    private Animator _animator;
    
    public ClimbingPlayerState(ClimbingPlayerStateData data)
    {
        _ladderDetector = data.LadderDetector;
        _ladderDetector.Triggered += HandleCollision;
        
        _playerTransform = data.PlayerTransform;

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
        _animator.SetBool("IsClimbing", true);
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
        _movement.SetInput(null);
        _animator.SetBool("IsClimbing", false);
    }

    private void HandleCollision(Collider2D other)
    {
        if (other.TryGetComponent(out LadderTrigger ladderTrigger))
        {
            _playerAttachmentEnterPoint = ladderTrigger.AttachmentPlayerEnterPoint.position;
            _playerAttachmentExitPoint = ladderTrigger.AttachmentPlayerExitPoint.position;
        }
    }
    
    ~ClimbingPlayerState()
    {
        _ladderDetector.Triggered -= HandleCollision;
    }
}
