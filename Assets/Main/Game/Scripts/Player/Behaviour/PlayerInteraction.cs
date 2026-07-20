using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] [Range(0f, float.MaxValue)]
    private float _interactionDelay = 1f;

    private bool _isInteractionDelay;
    private WaitForSeconds _interactionDelayWait;
    private IInteractionPlayerInput _input;

    private List<GameObject> _interactableObjects;

    public void Init(IInteractionPlayerInput input)
    {
        SetInput(input);
    }
    
    private void Awake()
    {
        _interactableObjects = new List<GameObject>();
        _interactionDelayWait = new WaitForSeconds(_interactionDelay);
    }


    private void Update()
    {
        if (!_isInteractionDelay)
        {
            Interact();
        }
    }
    

    public void SetInput(IInteractionPlayerInput input)
    {
        _input = input;
    }
    
    public void LaunchDelay()
    {
        StartCoroutine(SetInteractionDelay());
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
        if (_input != null && _input.IsInteractionButtonPressed())
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