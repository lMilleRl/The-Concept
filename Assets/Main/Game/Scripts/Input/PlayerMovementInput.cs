using UnityEngine;

public class PlayerMovementInput : MonoBehaviour, IMoveInputHandler
{
    public Vector2 GetMovementInput()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
