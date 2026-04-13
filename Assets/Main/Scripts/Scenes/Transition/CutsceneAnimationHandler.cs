using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneAnimationHandler : MonoBehaviour
{
    [SerializeField] private Image _currentImage;
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private TextBoxHandler _textBoxHandler;
    
    public IEnumerator PlayCutscene(CutsceneData cutsceneData)
    {
        _currentImage.sprite = cutsceneData.OwnSprite;
        _currentImage.SetNativeSize();
        _currentImage.rectTransform.anchoredPosition = Vector2.zero;

        float targetY = _currentImage.rectTransform.rect.height;

        _currentImage.rectTransform.DOAnchorPosY(targetY, cutsceneData.ScrollDurationInSec).SetEase(cutsceneData.ScrollSpriteEase);
        _textBoxHandler.ShowText(cutsceneData.OwnTextInfo, cutsceneData.PausePerPageInSec);

        float textDuration = (cutsceneData.PausePerPageInSec + cutsceneData.OwnTextInfo.TextAppearDuration)
            * _textBoxHandler.GetPagesCount();
        
        float totalDuration = Mathf.Max(textDuration, cutsceneData.ScrollDurationInSec);
        
        yield return new WaitForSeconds(totalDuration);
    }
}
