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

    public Tween CurrentTween { get; private set; }

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

    public void Show()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        CurrentTween = _canvasGroup.DOFade(1f, _appearanceDuration);
    }

    public void Hide()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        CurrentTween = _canvasGroup.DOFade(0f, _fadingDuration);
    }
}