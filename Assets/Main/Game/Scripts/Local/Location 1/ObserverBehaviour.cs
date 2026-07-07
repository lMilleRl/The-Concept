using DG.Tweening;
using UnityEngine;

public class ObserverBehaviour : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Transform _playerTransform;
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

        Vector3 startPos = GetTargetObserverPos();
        startPos.y += _startPosOffsetY;
        transform.position = startPos;
    }

    private void UpdateMove()
    {
        var targetPos = GetTargetObserverPos();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
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

    private Vector3 GetTargetObserverPos()
    {
        Vector3 targetPos = _camera.ViewportToWorldPoint(new Vector3(_viewportTargetPosition.x, _viewportTargetPosition.y, 0));
        targetPos.z = 0f;
        targetPos.x = _playerTransform.position.x;
        return targetPos;
    }
}
