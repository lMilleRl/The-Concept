using System;
using UnityEngine;

public class PlayerSpritesSwitcher : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private RuntimeAnimatorController _newController;

    private void OnEnable()
    {
        
    }

    public void Switch()
    {
        _playerAnimator.runtimeAnimatorController = _newController;
    }
}
