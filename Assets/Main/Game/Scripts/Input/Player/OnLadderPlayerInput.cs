using UnityEngine;

public class OnLadderPlayerInput : IMoveInput
{
    public bool IsInputEnabled { get; set; }
    public Vector2 GetMovementInput()
    {
        return new Vector2(0, Input.GetAxis("Vertical"));
    }

    public Vector2 GetRawMovementInput()
    {
        return new Vector2(0, Input.GetAxisRaw("Vertical"));
    }
}
