using DG.Tweening;
using UnityEngine;

public class ObserverBehaviour : MonoBehaviour
{
    [SerializeField] private float _escapeDuration = 0.4f;
    [SerializeField] private float _hiddenOffsetY = -3f;
    [SerializeField] private Ease _escapeEase = Ease.InQuad;

    private bool _isEscaping;

    public void Escape()
    {
        if (_isEscaping) return;
        _isEscaping = true;

        Vector3 hidePos = transform.position;
        hidePos.y += _hiddenOffsetY;

        transform.DOMove(hidePos, _escapeDuration)
            .SetEase(_escapeEase)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                _isEscaping = false;
            });
    }
}
