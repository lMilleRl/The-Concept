using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomToggle : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    private Bloom _bloom;

    private Bloom BloomComponent
    {
        get
        {
            if (_bloom == null && _volume != null && _volume.profile != null)
                _volume.profile.TryGet(out _bloom);

            return _bloom;
        }
    }

    public void SetActive(bool active)
    {
        if (BloomComponent != null)
            BloomComponent.active = active;
    }

    public void Enable() => SetActive(true);

    public void Disable() => SetActive(false);
}
