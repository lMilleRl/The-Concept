using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] [Range(0f, float.MaxValue)]
    private float _interactionDelay = 1f;

    [SerializeField] private KeyCode _keyToInteract;

    private bool _isInteractionDelay;
    private WaitForSeconds _interactionDelayWait;

    private List<GameObject> _interactableObjects;

    private void Awake()
    {
        _interactableObjects = new List<GameObject>();
        _interactionDelayWait = new WaitForSeconds(_interactionDelay);
    }

    private void OnEnable()
    {
        StartCoroutine(SetInteractionDelay());
    }

    private void Update()
    {
        if (!_isInteractionDelay)
        {
            Interact();
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
        if (Input.GetKeyDown(_keyToInteract))
        {
            if (TryGetInteractableObjByMinDistance(out GameObject objToInteract))
                if (objToInteract.TryGetComponent(out IInteractable interaction))
                {
                    var interactions = objToInteract.GetComponents<IInteractable>();
                    foreach (var i in interactions)
                    {
                        if (i is MonoBehaviour interactionComponent)
                        {
                            if (interactionComponent.enabled)
                                i.Activate();
                        }
                    }
                }
        }
    }

    private IEnumerator SetInteractionDelay()
    {
        _isInteractionDelay = true;
        yield return _interactionDelayWait;
        _isInteractionDelay = false;
    }
}