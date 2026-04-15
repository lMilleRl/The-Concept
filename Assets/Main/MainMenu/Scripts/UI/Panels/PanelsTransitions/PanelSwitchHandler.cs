using UnityEngine;

public class PanelSwitchHandler : MonoBehaviour
{
    [SerializeField] private Panel _panel;
    [SerializeField] private Panel _toPanel;

    private bool _isSwitchingComplete;

    private void Start()
    {
        _isSwitchingComplete = true;
    }

    public void SwitchPanel()
    {
        if (_isSwitchingComplete)
        {
            var fadingAnim = _panel.HideWithAnimation();

            fadingAnim.onComplete += StartOtherPanelAppearanceAnim;

            _isSwitchingComplete = false;
        }
    }

    private void StartOtherPanelAppearanceAnim()
    {
        var appearanceAnim = _toPanel.ShowWithAnimation();
        appearanceAnim.onComplete = () => _isSwitchingComplete = true;
    }
}