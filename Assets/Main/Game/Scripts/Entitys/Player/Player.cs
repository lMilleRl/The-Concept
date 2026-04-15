using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [Range(0f, float.MaxValue)] [SerializeField]
    private float _moveSpeed;

    [SerializeField] private KeyCode _keyToInteract;

    private Rigidbody2D _rigidbody2D;

    private List<GameObject> _interactableObjects;

    private bool _canUpdate = true;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _interactableObjects = new List<GameObject>();
    }

    private void Update()
    {
        if (_canUpdate)
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        if (_canUpdate)
        {
            Move();
        }
        else
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactableObject))
        {
            _interactableObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _interactableObjects.Remove(other.gameObject);
    }
    
    private void Move()
    {
        float velocityX, velocityY;
        velocityX = Input.GetAxis("Horizontal") * _moveSpeed;
        velocityY = Input.GetAxis("Vertical") * _moveSpeed;
        _rigidbody2D.velocity = new Vector2(velocityX, velocityY);
    }

    public void StopUpdate() => _canUpdate = false;
    public void ResumeUpdate() => _canUpdate = true;

    private bool TryGetInteractableObjByMinDistance(out GameObject minDistanceObj)
    {
        minDistanceObj = null;
        if (_interactableObjects.Count == 0) return false;

        minDistanceObj = _interactableObjects[0];
        var minSqrDistance = (minDistanceObj.transform.position - transform.position).sqrMagnitude;
        for (int i = 1; i < _interactableObjects.Count; i++)
        {
            var nextObjSqrDistance = (_interactableObjects[i].transform.position - transform.position).sqrMagnitude;
            if (minSqrDistance > nextObjSqrDistance)
            {
                minDistanceObj = _interactableObjects[i];
                minSqrDistance = nextObjSqrDistance;
            }
        }

        return true;
    }

    private void Interact()
    {
        if (Input.GetKeyDown(_keyToInteract) && TryGetInteractableObjByMinDistance(out GameObject objToInteract))
        {
            if (objToInteract.TryGetComponent(out IInteractable interaction))
                interaction.Activate();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}