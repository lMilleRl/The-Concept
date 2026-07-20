using UnityEngine;

public class PlayerMovementInput : MonoBehaviour, IMoveInput
{
    public bool IsInputEnabled { get; set; } = true;

    public Vector2 GetMovementInput()
    {
        if (!IsInputEnabled) return Vector2.zero;
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    
    public Vector2 GetRawMovementInput()
    {
        if (!IsInputEnabled) return Vector2.zero;
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
