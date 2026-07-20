using System;
using UnityEngine;

[RequireComponent(typeof(IMoveInput))]
public class PlayerMovement : MonoBehaviour
{
    [Range(0f, float.MaxValue)] [SerializeField]
    private float _moveSpeed;

    private Rigidbody2D _rigidbody2D;
    private IMoveInput _input;

    public Vector2 Velocity => _rigidbody2D.velocity;

    public void Init(IMoveInput input)
    {
        SetInput(input);
    }
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        _rigidbody2D.velocity = Vector2.zero;
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
            return;
        }
        
        _rigidbody2D.velocity = _input.GetMovementInput() * _moveSpeed;
    }
}