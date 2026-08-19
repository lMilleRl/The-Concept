using UnityEngine;

public struct ClimbingPlayerStateData
{
    public ITriggerDetector LadderDetector;
    public GameObject PlayerCollisionsDetector;
    public int IgnoreGroundLayer;
    public Transform PlayerTransform;
    public IPlayerMovement PlayerMovement;
    public IMoveInput ClimbingMoveInput;
    public Animator PlayerAnimator;
    public Rigidbody2D PlayerRigidBody;
    public Collider2D PlayerCollider;
    public SpriteRenderer PlayerSpriteRenderer;
    public MovementStateData MovementStateData;
    public IPlayerMovementStateReceiver MovementStateReceiver;

    public ClimbingPlayerStateData(
        ITriggerDetector ladderDetector,
        GameObject playerCollisionsDetector,
        int ignoreGroundLayer,
        Transform playerTransform,
        IPlayerMovement playerMovement,
        IMoveInput climbingMoveInput,
        Animator playerAnimator,
        Rigidbody2D playerRigidBody,
        Collider2D playerCollider,
        SpriteRenderer playerSpriteRenderer,
        MovementStateData movementStateData,
        IPlayerMovementStateReceiver movementStateReceiver)
    {
        LadderDetector = ladderDetector;
        PlayerCollisionsDetector = playerCollisionsDetector;
        IgnoreGroundLayer = ignoreGroundLayer;
        PlayerTransform = playerTransform;
        PlayerMovement = playerMovement;
        ClimbingMoveInput = climbingMoveInput;
        PlayerAnimator = playerAnimator;
        PlayerRigidBody = playerRigidBody;
        PlayerCollider = playerCollider;
        PlayerSpriteRenderer = playerSpriteRenderer;
        MovementStateData = movementStateData;
        MovementStateReceiver = movementStateReceiver;
    }
}