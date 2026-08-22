using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractionActivator : MonoBehaviour
{
    [SerializeField] private InteractionBase _playerInteraction;
    private IPlayerMovement _playerMovement;

    [Range(0f, float.MaxValue)] [SerializeField] private float _distanceFromPlayer;

    private IMoveInput _moveInput;
    
    private event Action<Collider2D> _onTriggerEnter;
    private event Action<Collider2D> _onTriggerExit;

    private void OnValidate()
    {
        transform.localPosition = new Vector3(_distanceFromPlayer, 0, 0);
    }

    public void Init(IMoveInput moveInput, IPlayerMovement playerMovement)
    {
        _moveInput = moveInput;
        _playerMovement = playerMovement;
    }
    
    private void Awake()
    {
        transform.localPosition = new Vector3(_distanceFromPlayer, 0, 0);
    }

    private void OnEnable()
    {
        _onTriggerEnter += _playerInteraction.OnInteractionTriggerEnter;
        _onTriggerExit += _playerInteraction.OnInteractionTriggerExit;
    }

    private void OnDisable()
    {
        _onTriggerEnter -= _playerInteraction.OnInteractionTriggerEnter;
        _onTriggerExit -= _playerInteraction.OnInteractionTriggerExit;
    }

    private void Update()
    {
        TurnToPlayerMove();
    }

    private void TurnToPlayerMove()
    {
        if (_moveInput == null)
            return;

        var rawMoveInput = _moveInput.GetRawMovementInput();
        bool isPlayerInputMovingKeys = rawMoveInput.x != 0 || rawMoveInput.y != 0;
        if (isPlayerInputMovingKeys)
        {
            Vector2 intendedDirection = _playerMovement.IntendedDirection;
            float angleToTurn = Mathf.Atan2(intendedDirection.y, intendedDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleToTurn);
            transform.localPosition = intendedDirection * _distanceFromPlayer;
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