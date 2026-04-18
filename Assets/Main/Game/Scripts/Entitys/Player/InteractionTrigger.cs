using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour
{
    [SerializeField] private Player _player;

    [Range(0f, float.MaxValue)] [SerializeField] private float _distanceFromPlayer;
    
    private event Action<Collider2D> _onTriggerEnter;
    private event Action<Collider2D> _onTriggerExit;

    private void OnValidate()
    {
        transform.localPosition = new Vector3(_distanceFromPlayer, 0, 0);
    }

    private void Awake()
    {
        transform.localPosition = new Vector3(_distanceFromPlayer, 0, 0);
    }

    private void OnEnable()
    {
        _onTriggerEnter += _player.OnInteractionTriggerEnter;
        _onTriggerExit += _player.OnInteractionTriggerExit;
    }

    private void OnDisable()
    {
        _onTriggerEnter -= _player.OnInteractionTriggerEnter;
        _onTriggerExit -= _player.OnInteractionTriggerExit;
    }

    private void Update()
    {
        TurnToPlayerMove();
    }

    private void TurnToPlayerMove()
    {
        bool isPlayerInputMovingKeys = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        if (isPlayerInputMovingKeys)
        {
            float angleToTurn = Mathf.Atan2(_player.Velocity.y, _player.Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleToTurn);
            transform.localPosition = _player.Velocity.normalized * _distanceFromPlayer;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _onTriggerExit?.Invoke(other);
    }
}