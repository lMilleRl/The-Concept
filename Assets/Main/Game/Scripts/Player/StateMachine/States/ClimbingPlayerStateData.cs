using UnityEngine;

public struct ClimbingPlayerStateData
{
    public ITriggerDetector LadderDetector;
    public Transform PlayerTransform;
    public PlayerMovement PlayerMovement;
    public IMoveInput ClimbingMoveInput;
    public Animator PlayerAnimator;
    public Rigidbody2D PlayerRigidBody;
    public Collider2D PlayerCollider;

    public ClimbingPlayerStateData(
        ITriggerDetector ladderDetector,
        Transform playerTransform,
        PlayerMovement playerMovement,
        IMoveInput climbingMoveInput,
        Animator playerAnimator,
        Rigidbody2D playerRigidBody,
        Collider2D playerCollider)
    {
        LadderDetector = ladderDetector;
        PlayerTransform = playerTransform;
        PlayerMovement = playerMovement;
        ClimbingMoveInput = climbingMoveInput;
        PlayerAnimator = playerAnimator;
        PlayerRigidBody = playerRigidBody;
        PlayerCollider = playerCollider;
    }
}


