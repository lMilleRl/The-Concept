using DG.Tweening;
using UnityEngine;

public class ObserverBehaviour : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Vector2 _viewportTargetPosition = new Vector2(0.5f, 0.05f);
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _escapeDuration = 0.4f;
    [SerializeField] private float _hiddenOffsetY = -3f;
    [SerializeField] private float _startPosOffsetY = -3f;
    [SerializeField] private Ease _escapeEase = Ease.InQuad;

    private bool _isEscaping;

    public void Activate()
    {
        gameObject.SetActive(true);

        Vector3 startPos = GetViewportWorldPosition();
        startPos.y += _startPosOffsetY;
        transform.position = startPos;
    }

    private void UpdateMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetViewportWorldPosition(), _moveSpeed * Time.deltaTime);
    }
    
    private void Update()
    {
        if (!_isEscaping)
            UpdateMove();
        if (_playerMovement.Velocity.y < 0 && !_isEscaping)
            Escape();
    }

    private void Escape()
    {
        _isEscaping = true;
        Vector3 hidePos = transform.position;
        hidePos.y += _hiddenOffsetY;

        transform.DOMove(hidePos, _escapeDuration)
            .SetEase(_escapeEase)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private Vector3 GetViewportWorldPosition()
    {
        Vector3 worldPos = _camera.ViewportToWorldPoint(new Vector3(_viewportTargetPosition.x, _viewportTargetPosition.y, 0f));
        worldPos.z = 0f;
        return worldPos;
    }
}
