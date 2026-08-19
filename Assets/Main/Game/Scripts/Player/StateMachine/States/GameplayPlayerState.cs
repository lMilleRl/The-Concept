using UnityEngine;

public class GameplayPlayerState : MovementState
{
    private readonly IPlayerMovement _movement;
    private readonly IPlayerInteraction _interaction;
    private readonly IInteractionPlayerInput _interactionInput;
    private readonly IMoveInput _moveInput;
    private readonly IPlayerMovementStateReceiver _movementStateReceiver;

    public GameplayPlayerState(GameplayPlayerStateData data) : base(data.MovementStateData)
    {
        _movement = data.PlayerMovement;
        _interaction = data.PlayerInteraction;
        _interactionInput = data.InteractionInput;
        _moveInput = data.MoveInput;
        _movementStateReceiver = data.MovementStateReceiver;
    }

    public override void Enter()
    {
        _movement.SetInput(_moveInput);
        _interaction.SetInput(_interactionInput);
        _interaction.LaunchDelay();
        _movementStateReceiver.SetMovementState(PlayerMovementStateType.Walking);
        EnterMovementEffects();
    }

    public override void Update()
    {
        UpdateMovementEffects();
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        _movement.SetInput(null);
        _interaction.SetInput(null);
        _movementStateReceiver.SetMovementState(PlayerMovementStateType.Idle);
        ExitMovementEffects();
    }

    protected override bool IsMoving()
    {
        return _moveInput != null && _movement.IsMovingByInput;
    }
}