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
        if (cutscene.IsScrolling)
        {
            PlayImageScroll(cutscene.OwnSprite, cutscene.ScrollDurationInSec, cutscene.ScrollSpriteEase);
        }
        else
        {
            PlayStaticImage(cutscene.OwnSprite);
        }

        _textBoxHandler.ShowText(cutscene.OwnTextInfo, cutscene.PausePerPageInSec);

        float textDuration = (cutscene.PausePerPageInSec + cutscene.OwnTextInfo.TextAppearDuration)
                             * _textBoxHandler.GetPagesCount();
        float imageAnimationDuration = cutscene.IsScrolling ? cutscene.ScrollDurationInSec : 0f;

        float totalDuration = Mathf.Max(textDuration, imageAnimationDuration);

        float durationWithFading = totalDuration + cutscene.UIFadeOutDurationInSec;
        StartCoroutine(ResetCutsceneImagePosAfter(durationWithFading));

        yield return new WaitForSeconds(totalDuration);
    }

    // чтобы корректно отыграть скролл, нужно, чтобы пивот изображения был (0.5, 1)
    private void PlayImageScroll(Sprite sprite, float duration, Ease easeType)
    {
        FitImageToMaskWidth(sprite);
        _currentImage.sprite = sprite;

        // предполагается, что изначальная позиция изображения - корректная
        float targetY = _initImagePos.y + _currentImage.rectTransform.rect.height;

        _currentImage.rectTransform.DOAnchorPosY(targetY, duration)
            .SetEase(easeType);
    }

    private void PlayStaticImage(Sprite sprite)
    {
        FitImageToMaskHeight(sprite);
        _currentImage.sprite = sprite;
        FitImageToMask();
    }

    // Работает для image с anchors (0,0)-(1,1) (stretched) и pivot (0.5, 1).
    // При других настройках формулу нужно пересчитать.
    private void FitImageToMask()
    {
        _currentImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void FitImageToMaskWidth(Sprite sprite)
    {
        float maskWidth = _mask.rectTransform.rect.width;
        float newHeight = maskWidth * GetHeightToWidthRatio(sprite);
        SetImageSize(maskWidth, newHeight);
    }

    private void FitImageToMaskHeight(Sprite sprite)
    {
        float maskHeight = _mask.rectTransform.rect.height;
        float newWidth = maskHeight * GetWidthToHeightRaio(sprite);
        SetImageSize(newWidth, maskHeight);
    }

    private float GetHeightToWidthRatio(Sprite sprite) => sprite.rect.height / sprite.rect.width;
    private float GetWidthToHeightRaio(Sprite sprite) => sprite.rect.width / sprite.rect.height;

    private void SetImageSize(float newWidth, float newHeight)
    {
        var rt = _currentImage.rectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }

    private IEnumerator ResetCutsceneImagePosAfter(float secondsFoWaiting)
    {
        yield return new WaitForSeconds(secondsFoWaiting);
        _currentImage.rectTransform.anchoredPosition = _initImagePos;
    }
}