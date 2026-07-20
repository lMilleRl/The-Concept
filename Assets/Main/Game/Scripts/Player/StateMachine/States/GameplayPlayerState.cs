using UnityEngine;

public class GameplayPlayerState : State
{
    private PlayerMovement _movement;
    private PlayerInteraction _interaction;
    private IInteractionPlayerInput _interactionInput;
    private IMoveInput _moveInput;
    
    public GameplayPlayerState(GameplayPlayerStateData data)
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
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        _movement.SetInput(null);
        _interaction.SetInput(null);
    }
}

