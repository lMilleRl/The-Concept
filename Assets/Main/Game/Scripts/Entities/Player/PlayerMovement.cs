using System;
using UnityEngine;

[RequireComponent(typeof(IMoveInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Range(0f, float.MaxValue)] [SerializeField]
    private float _moveSpeed;

    private Vector2 _moveDirection;
    private Rigidbody2D _rigidbody2D;
    private IMoveInputHandler _inputHandler;

    public Vector2 Velocity => _rigidbody2D.velocity;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _inputHandler = GetComponent<IMoveInputHandler>();
    }

    private void OnDisable()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rigidbody2D.velocity = _inputHandler.GetMovementInput() * _moveSpeed;
    }
}