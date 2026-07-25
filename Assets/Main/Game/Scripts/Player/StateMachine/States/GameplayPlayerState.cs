using UnityEngine;

public class GameplayPlayerState : MovementState
{
    private readonly PlayerMovement _movement;
    private readonly PlayerInteraction _interaction;
    private readonly IInteractionPlayerInput _interactionInput;
    private readonly IMoveInput _moveInput;

    public GameplayPlayerState(GameplayPlayerStateData data) : base(data.MovementStateData)
    {
        _movement = data.PlayerMovement;
        _interaction = data.PlayerInteraction;
        _interactionInput = data.InteractionInput;
        _moveInput = data.MoveInput;
    }

    public override void Enter()
    {
        _movement.SetInput(_moveInput);
        _interaction.SetInput(_interactionInput);
        _interaction.LaunchDelay();
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
        ExitMovementEffects();
    }

    protected override bool IsMoving()
    {
        return _moveInput != null && _movement.Velocity.sqrMagnitude > 0.001f;
    }
}