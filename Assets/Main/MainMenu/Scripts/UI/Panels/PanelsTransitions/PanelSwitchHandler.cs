using UnityEngine;

public class PanelSwitchHandler : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroupAnimator fadeCanvasGroupAnimator;
    [SerializeField] private FadeCanvasGroupAnimator toFadeCanvasGroupAnimator;

    private bool _isSwitchingComplete;

    private void Start()
    {
        _isSwitchingComplete = true;
    }

    public void SwitchPanel()
    {
        if (_isSwitchingComplete)
        {
            var fadingAnim = fadeCanvasGroupAnimator.HideWithAnimation();

            fadingAnim.onComplete += StartOtherPanelAppearanceAnim;

            _isSwitchingComplete = false;
        }
    }

    private void StartOtherPanelAppearanceAnim()
    {
        var appearanceAnim = toFadeCanvasGroupAnimator.ShowWithAnimation();
        appearanceAnim.onComplete = () => _isSwitchingComplete = true;
    }
}