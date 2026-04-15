using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Panel : MonoBehaviour
{
    [SerializeField] private bool _isHiddenOnStart;

    [Range(0f, float.MaxValue)] [SerializeField]
    private float _fadingDuration;

    [Range(0f, float.MaxValue)] [SerializeField]
    private float _appearanceDuration;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_isHiddenOnStart)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void Show() => ShowWithAnimation();
    
    public Tween ShowWithAnimation()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        return _canvasGroup.DOFade(1f, _appearanceDuration);
    }

    public void Hide() => HideWithAnimation();
    
    public Tween HideWithAnimation()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        return _canvasGroup.DOFade(0f, _fadingDuration);
    }
}