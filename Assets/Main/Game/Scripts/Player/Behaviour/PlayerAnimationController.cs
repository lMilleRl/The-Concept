using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, IPlayerMovementStateReceiver
{
    private static readonly int MoveXParam = Animator.StringToHash("MoveX");
    private static readonly int MoveYParam = Animator.StringToHash("MoveY");
    private static readonly int LastDirectionParam = Animator.StringToHash("LastDirection");
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
    private static readonly int IsClimbingParam = Animator.StringToHash("IsClimbing");

    [SerializeField] private Animator _animator;

    private IPlayerMovement _movement;
    private FacingDirection _facingDirection = FacingDirection.Down;
    private PlayerMovementStateType _currentState = PlayerMovementStateType.Idle;

    public void Init(IPlayerMovement movement)
    {
        _movement = movement;
    }

    public void SetMovementState(PlayerMovementStateType state)
    {
        _currentState = state;

        _animator.SetBool(IsMovingParam, state == PlayerMovementStateType.Walking);
        _animator.SetBool(IsClimbingParam, state == PlayerMovementStateType.Climbing);

        if (state != PlayerMovementStateType.Walking)
        {
            _animator.SetFloat(MoveXParam, 0f);
            _animator.SetFloat(MoveYParam, 0f);
        }
    }

    private void Update()
    {
        if (_movement == null)
            return;

        if (_currentState == PlayerMovementStateType.Walking)
        {
            UpdateWalking();
        }
        else if (_currentState == PlayerMovementStateType.Idle)
        {
            UpdateIdle();
        }
    }

    private void UpdateWalking()
    {
        Vector2 intendedDirection = _movement.IntendedDirection;
        bool isMoving = _movement.IsMovingByInput;

        if (intendedDirection != Vector2.zero)
        {
            _facingDirection = ResolveFacingDirection(intendedDirection);
            _animator.SetInteger(LastDirectionParam, (int)_facingDirection);
        }

        if (isMoving)
        {
            _animator.SetFloat(MoveXParam, intendedDirection.x);
            _animator.SetFloat(MoveYParam, intendedDirection.y);
        }
        else
        {
            _animator.SetFloat(MoveXParam, 0f);
            _animator.SetFloat(MoveYParam, 0f);
        }

        _animator.SetBool(IsMovingParam, isMoving);
    }

    private void UpdateIdle()
    {
        Vector2 inputDirection = _movement.IntendedDirection;

        if (inputDirection != Vector2.zero)
        {
            _facingDirection = ResolveFacingDirection(inputDirection);
            _animator.SetInteger(LastDirectionParam, (int)_facingDirection);
        }
    }

    private static FacingDirection ResolveFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? FacingDirection.Right : FacingDirection.Left;

        return direction.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }
}
