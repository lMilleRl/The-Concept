using UnityEngine;

public class PlayerSpritesSwitcher : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private RuntimeAnimatorController _newController;

    public void Switch()
    {
        _playerAnimator.runtimeAnimatorController = _newController;
    }
}
