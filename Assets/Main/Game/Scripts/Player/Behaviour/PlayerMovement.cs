using System;
using UnityEngine;

[RequireComponent(typeof(IMoveInput))]
public class PlayerMovement : MonoBehaviour, IAnimationMovementSource, IPlayerMovement
{
    private const float MovementThreshold = 0.05f;

    [Range(0f, float.MaxValue)] [SerializeField]
    private float _moveSpeed;

    [SerializeField] private Rigidbody2D _rigidbody2D;
    private IMoveInput _input;
    private Vector2 _intendedDirection;

    public Vector2 Velocity => ActualVelocity;

    public Vector2 IntendedDirection => _intendedDirection;
    public Vector2 ActualVelocity => _rigidbody2D.velocity;

    public bool IsMovingByInput => IsMovingByIntent();

    private bool IsMovingByIntent()
    {
        if (_intendedDirection == Vector2.zero)
            return false;

        float movementAlongIntent = Vector2.Dot(_rigidbody2D.velocity, _intendedDirection);
        return movementAlongIntent > MovementThreshold;
    }

    public void Init(IMoveInput input)
    {
        SetInput(input);
    }
    
    private void OnDisable()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _intendedDirection = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetInput(IMoveInput input)
    {
        _input = input;
    }

    private void Move()
    {
        if (_input == null)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _intendedDirection = Vector2.zero;
            return;
        }

        _intendedDirection = _input.GetMovementInput().normalized;
        _rigidbody2D.velocity = _intendedDirection * _moveSpeed;
    }
}