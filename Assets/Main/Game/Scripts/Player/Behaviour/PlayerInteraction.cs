using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : InteractionBase
{
    [SerializeField] [Range(0f, float.MaxValue)]
    private float _interactionDelay = 1f;

    private bool _isInteractionDelay;
    private WaitForSeconds _interactionDelayWait;
    private IInteractionPlayerInput _input;

    private readonly List<IInteractable[]> _interactableComponents = new();

    public override bool CanInteract => HasAnyInteractableAvailable() && !_isInteractionDelay && _input != null;

    private bool HasAnyInteractableAvailable()
    {
        for (int i = 0; i < _interactableComponents.Count; i++)
        {
            if (HasAnyEnabledInteractable(_interactableComponents[i]))
                return true;
        }

        return false;
    }

    private static bool HasAnyEnabledInteractable(IInteractable[] interactables)
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i] is MonoBehaviour mono && mono.enabled)
                return true;
        }

        return false;
    }

    public void Init(IInteractionPlayerInput input)
    {
        SetInput(input);
    }
    
    private void Awake()
    {
        _interactionDelayWait = new WaitForSeconds(_interactionDelay);
    }


    private void Update()
    {
        if (!_isInteractionDelay)
        {
            Interact();
        }
    }
    

    public override void SetInput(IInteractionPlayerInput input)
    {
        _input = input;
    }
    
    public override void LaunchDelay()
    {
        StartCoroutine(SetInteractionDelay());
    }
    
    public override void OnInteractionTriggerEnter(Collider2D other)
    {
        var interactables = other.gameObject.GetComponents<IInteractable>();
        if (interactables.Length > 0)
            _interactableComponents.Add(interactables);
    }

    public override void OnInteractionTriggerExit(Collider2D other)
    {
        var interactables = other.gameObject.GetComponents<IInteractable>();
        for (int i = 0; i < _interactableComponents.Count; i++)
        {
            if (ReferenceEquals(((MonoBehaviour)_interactableComponents[i][0]).transform, other.transform))
            {
                _interactableComponents.RemoveAt(i);
                return;
            }
        }
    }

    private bool TryGetInteractablesByMinDistance(out IInteractable[] closestInteractables)
    {
        closestInteractables = null;
        if (_interactableComponents.Count == 0) return false;

        int closestIndex = 0;
        var firstTransform = ((MonoBehaviour)_interactableComponents[0][0]).transform;
        var minSqrDistance = (firstTransform.position - transform.position).sqrMagnitude;
        for (int i = 1; i < _interactableComponents.Count; i++)
        {
            var nextTransform = ((MonoBehaviour)_interactableComponents[i][0]).transform;
            var nextSqrDistance = (nextTransform.position - transform.position).sqrMagnitude;
            if (minSqrDistance > nextSqrDistance)
            {
                closestIndex = i;
                minSqrDistance = nextSqrDistance;
            }
        }

        closestInteractables = _interactableComponents[closestIndex];
        return true;
    }

    private void Interact()
    {
        if (_input != null && _input.IsInteractionButtonPressed())
        {
            if (TryGetInteractablesByMinDistance(out var interactables) &&
                HasAnyEnabledInteractable(interactables))
            {
                for (int i = 0; i < interactables.Length; i++)
                {
                    if (interactables[i] is MonoBehaviour mono && mono.enabled)
                        interactables[i].Activate();
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