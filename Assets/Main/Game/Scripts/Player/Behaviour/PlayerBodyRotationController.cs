using UnityEngine;

public class PlayerBodyRotationController : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (_playerMovement == null)
            return;

        Vector2 direction = _playerMovement.IntendedDirection;
        if (direction == Vector2.zero)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
