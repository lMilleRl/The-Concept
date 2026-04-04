using DG.Tweening;
using TMPro;

public static class TextMeshProUGUIExtensions
{
    public static Tween DOText(this TextMeshProUGUI text, float duration)
    {
        text.maxVisibleCharacters = 0;
        return DOTween.To(() => text.maxVisibleCharacters, x => text.maxVisibleCharacters = x,
            text.text.Length, duration);
    }

    public static Tween DOText(this TextMeshProUGUI text, int startCharIndex, int endCharIndex, float duration)
    {
        var charCount = endCharIndex + 1;
        text.maxVisibleCharacters = startCharIndex;
        return DOTween.To(() => text.maxVisibleCharacters, x => text.maxVisibleCharacters = x,
            charCount, duration);
    }

    public static Tween DOPage(this TextMeshProUGUI text, TMP_PageInfo pageInfo, float duration)
    {
        return text.DOText(pageInfo.firstCharacterIndex, pageInfo.lastCharacterIndex, duration);
    }
}