using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Range(0f, float.MaxValue)] [SerializeField]
    private float _interactDelayAfterStopUpdateInSec;

    [Range(0f, float.MaxValue)] [SerializeField]
    private float _moveSpeed;

    [SerializeField] private KeyCode _keyToInteract;

    private Vector2 _moveDirection;
    private Rigidbody2D _rigidbody2D;
    private List<GameObject> _interactableObjects;
    private bool _canUpdate = true;
    private bool _canInteract = true;
    
    public Vector2 Velocity => _rigidbody2D.velocity;

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

    public void OnInteractionTriggerEnter(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactableObject))
        {
            _interactableObjects.Add(other.gameObject);
        }
    }

    public void OnInteractionTriggerExit(Collider2D other)
    {
        _interactableObjects.Remove(other.gameObject);
    }

    private void Move()
    {
        float moveX, moveY;
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        _rigidbody2D.velocity = new Vector2(moveX, moveY) * _moveSpeed;
    }

    public void StopUpdate()
    {
        _canInteract = false;
        _canUpdate = false;
    }

    public void ResumeUpdate()
    {
        StartCoroutine(InteractDelay());
        _canUpdate = true;
    }

    private IEnumerator InteractDelay()
    {
        yield return new WaitForSeconds(_interactDelayAfterStopUpdateInSec);
        _canInteract = true;
    }

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
        bool _isInteractingNow = _canInteract && Input.GetKeyDown(_keyToInteract);
        if (_isInteractingNow)
        {
            if (TryGetInteractableObjByMinDistance(out GameObject objToInteract))
                if (objToInteract.TryGetComponent(out IInteractable interaction))
                    interaction.Activate();
        }
    }
}