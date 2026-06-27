using UnityEngine;

public class ComponentsSwitcher : MonoBehaviour
{
    [SerializeField] private Behaviour[] _toDeactivation;
    [SerializeField] private Behaviour[] _toActivation;

    public void Switch()
    {
        foreach (var b in _toDeactivation) b.enabled = false;
        foreach (var b in _toActivation) b.enabled = true;
    }
}
