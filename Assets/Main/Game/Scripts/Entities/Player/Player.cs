using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerInteraction _interaction;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _interaction = GetComponent<PlayerInteraction>();
    }

    public void StopUpdate()
    {
        _movement.enabled = false;
        _interaction.enabled = false;
    }

    public void ResumeUpdate()
    {
        _movement.enabled = true;
        _interaction.enabled = true;
    }
}