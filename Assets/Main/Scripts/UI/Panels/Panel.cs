using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [Range(0f, float.MaxValue)] [SerializeField]
    private float _fadingDuration;

    [Range(0f, float.MaxValue)] [SerializeField]
    private float _appearanceDuration;

    private List<Image> _childrenImages;
    private List<float> _originalImagesAlpha;

    private List<TextMeshProUGUI> _childrenTexts;
    private List<float> _originalTextsAlpha;
    
    private bool _isInit = false;

    private void Start()
    {
        Init();
    }
    
    public Sequence CurrentSequence { get; private set; }

    public void Init()
    {
        if (_isInit) return;
        InitImagesInfo();
        InitTextsInfo();

        _isInit = true;
        
        void InitImagesInfo()
        {
            _childrenImages = new List<Image>(gameObject.GetComponentsInChildren<Image>());
        
            _originalImagesAlpha = new List<float>(_childrenImages.Count);
            foreach (var image in _childrenImages)
            {
                _originalImagesAlpha.Add(image.color.a);
            }
        }
        void InitTextsInfo()
        {
            _childrenTexts = new List<TextMeshProUGUI>(gameObject.GetComponentsInChildren<TextMeshProUGUI>());
        
            _originalTextsAlpha = new List<float>(_childrenTexts.Count);
            foreach (var text in _childrenTexts)
            {
                _originalTextsAlpha.Add(text.color.a);
            }
        }
    }

    public void StartAnimSwitchActivity(bool isPanelActive)
    {
        CurrentSequence = DOTween.Sequence();
        if (isPanelActive)
        {
            FadeIn(_appearanceDuration);
        }
        else
        {
            FadeOut(_fadingDuration);
        }
    }

    private void FadeIn(float duration)
    {
        for (int i = 0; i < _childrenImages.Count; i++)
        {
            _childrenImages[i].DOKill();
            _childrenImages[i].color = new Color(_childrenImages[i].color.r, _childrenImages[i].color.g, _childrenImages[i].color.b, 0);
            CurrentSequence.Insert(0f, _childrenImages[i].DOFade(_originalImagesAlpha[i], duration));
        }
        for (int i = 0; i < _childrenTexts.Count; i++)
        {
            _childrenTexts[i].DOKill();
            _childrenTexts[i].color = new Color(_childrenTexts[i].color.r, _childrenTexts[i].color.g, _childrenTexts[i].color.b, 0);
            CurrentSequence.Insert(0f, _childrenTexts[i].DOFade(_originalTextsAlpha[i], duration));
        }
    }
    
    private void FadeOut(float duration)
    {
        for (int i = 0; i < _childrenImages.Count; i++)
        {
            _childrenImages[i].DOKill();
            CurrentSequence.Insert(0f, _childrenImages[i].DOFade(0f, duration));
        }
        for (int i = 0; i < _childrenTexts.Count; i++)
        {
            _childrenTexts[i].DOKill();
            CurrentSequence.Insert(0f, _childrenTexts[i].DOFade(0f, duration));
        }
    }
}