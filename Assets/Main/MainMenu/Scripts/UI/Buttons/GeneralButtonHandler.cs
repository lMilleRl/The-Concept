using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeneralButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    
    [Range(0f, 1f)] [SerializeField] private float _targetScale;
    [Range(0f, float.MaxValue)] [SerializeField] private float _durationChangingScale;
    [Range(0f, float.MaxValue)] [SerializeField] private float _durationScaleRestoration;
    
    [SerializeField] private Color _targetColor;
    [Range(0f, float.MaxValue)] [SerializeField] private float _durationChangingColor;
    [Range(0f, float.MaxValue)] [SerializeField] private float _durationColorRestoration;

    private Vector3 _originalScale;
    private Color _originalColor;

    private bool _isInit;
    
    private void OnEnable()
    {
        Init();
        _button.onClick.AddListener(ChangeScale);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ChangeScale);
        ResetToDefaultView();
    }
    
    private void Start()
    {
        Init();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _button.image.DOKill();
        _button.image.DOColor(_targetColor, _durationChangingColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _button.image.DOKill();
        _button.image.DOColor(_originalColor, _durationColorRestoration);
    }

    private void Init()
    {
        if (_isInit) return;
        
        _originalScale = _button.transform.localScale;
        _originalColor = _button.image.color;

        _isInit = true;
    }
    
    private void ChangeScale()
    {
        _button.transform.DOKill();
        
        var scaleChangingSequence = DOTween.Sequence();
        scaleChangingSequence.Append(_button.transform.DOScale(_targetScale, _durationChangingScale));
        scaleChangingSequence.Append(_button.transform.DOScale(_originalScale, _durationScaleRestoration));
    }

    private void ResetToDefaultView()
    {
        _button.image.DOKill();
        _button.image.color = _originalColor;
        _button.transform.localScale = _originalScale;
    }
}
