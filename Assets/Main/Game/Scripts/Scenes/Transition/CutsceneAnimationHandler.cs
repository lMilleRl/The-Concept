using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneAnimationHandler : MonoBehaviour
{
    [SerializeField] private Image _currentImage;
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private TextBoxHandler _textBoxHandler;

    private Vector2 _initImagePos;

    private void Awake()
    {
        _initImagePos = _currentImage.rectTransform.anchoredPosition;
    }
    
    public IEnumerator PlayCutscene(CutsceneData cutscene)
    {
        AdjustImageToFitSprite(cutscene.OwnSprite);
        _currentImage.sprite = cutscene.OwnSprite;

        float targetY = _initImagePos.y + _currentImage.rectTransform.rect.height;

        _currentImage.rectTransform.DOAnchorPosY(targetY, cutscene.ScrollDurationInSec)
            .SetEase(cutscene.ScrollSpriteEase);
        _textBoxHandler.ShowText(cutscene.OwnTextInfo, cutscene.PausePerPageInSec);

        float textDuration = (cutscene.PausePerPageInSec + cutscene.OwnTextInfo.TextAppearDuration)
                             * _textBoxHandler.GetPagesCount();

        float totalDuration = Mathf.Max(textDuration, cutscene.ScrollDurationInSec);

        float durationWithFading = totalDuration + cutscene.UIFadeOutDurationInSec;
        StartCoroutine(ResetCutsceneImagePosAfter(durationWithFading));
        
        yield return new WaitForSeconds(totalDuration);
    }

    private void AdjustImageToFitSprite(Sprite sprite)
    {
        float maskWidth = _mask.rectTransform.rect.width;
        float spriteAspect = sprite.rect.height / sprite.rect.width;
        float newHeight = maskWidth * spriteAspect;

        var rt = _currentImage.rectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }

    private IEnumerator ResetCutsceneImagePosAfter(float secondsFoWaiting)
    {
        yield return new WaitForSeconds(secondsFoWaiting);
        _currentImage.rectTransform.anchoredPosition = _initImagePos;
    }
}