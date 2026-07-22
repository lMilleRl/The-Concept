using UnityEngine;

public struct ClimbingPlayerStateData
{
    public ITriggerDetector LadderDetector;
    public GameObject PlayerCollisionsDetector;
    public int IgnoreGroundLayer;
    public Transform PlayerTransform;
    public PlayerMovement PlayerMovement;
    public IMoveInput ClimbingMoveInput;
    public Animator PlayerAnimator;
    public Rigidbody2D PlayerRigidBody;
    public Collider2D PlayerCollider;
    public SpriteRenderer PlayerSpriteRenderer;

    public ClimbingPlayerStateData(
        ITriggerDetector ladderDetector,
        GameObject playerCollisionsDetector,
        int ignoreGroundLayer,
        Transform playerTransform,
        PlayerMovement playerMovement,
        IMoveInput climbingMoveInput,
        Animator playerAnimator,
        Rigidbody2D playerRigidBody,
        Collider2D playerCollider,
        SpriteRenderer playerSpriteRenderer)
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
    }
}