using UnityEngine;

public interface IMoveInputHandler
{
    // a value of output axis is from 0 to 1
    Vector2 GetMovementInput();
}
