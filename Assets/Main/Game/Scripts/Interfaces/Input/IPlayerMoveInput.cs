using UnityEngine;

public interface IMoveInput : IPlayerInput
{
    // a value of output axis is from 0 to 1
    Vector2 GetMovementInput();
    
    Vector2 GetRawMovementInput();
}
