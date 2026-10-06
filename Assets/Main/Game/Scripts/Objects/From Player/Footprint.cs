using System;
using DG.Tweening;
using UnityEngine;

public class Footprint : MonoBehaviour, IPoolable<Footprint>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField, Min(0f)] private float _lifeTimeInSec;

    private Action<Footprint> _returnToPool;
    private SpriteRenderer _playerSpriteRenderer;
    private Color _initialColor;
    private Tween _fadeTween;
    private bool _isUnderPlayer;

    private void Awake()
    {
        _initialColor = _spriteRenderer.color;
    }

    private void OnEnable()
    {
        CancelInvoke(nameof(Release));
        _fadeTween?.Kill();
        _fadeTween = null;
        _spriteRenderer.color = _initialColor;
        _isUnderPlayer = false;

        float lifeTime = Mathf.Max(0f, _lifeTimeInSec);
        if (lifeTime == 0f)
        {
            Invoke(nameof(Release), 0f);
            return;
        }

        _fadeTween = _spriteRenderer.DOFade(0f, lifeTime)
            .SetEase(Ease.Linear)
            .OnComplete(Release);
        UpdateFadePauseState();
    }

    private void Update()
    {
        UpdateFadePauseState();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Release));
        _fadeTween?.Kill();
        _fadeTween = null;
        _isUnderPlayer = false;
        _spriteRenderer.color = _initialColor;
    }

    public void SetSprite(Sprite sprite, bool isMirrored)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.flipX = isMirrored;
    }

    public void SetPlayerSpriteRenderer(SpriteRenderer playerSpriteRenderer)
    {
        _playerSpriteRenderer = playerSpriteRenderer;
        UpdateFadePauseState();
    }

    public void InitForPool(Action<Footprint> returnToPool)
    {
        _returnToPool = returnToPool;
    }

    public void Release()
    {
        _returnToPool?.Invoke(this);
    }

    private void UpdateFadePauseState()
    {
        if (_fadeTween == null)
            return;

        bool isUnderPlayer = _playerSpriteRenderer != null &&
            _playerSpriteRenderer.bounds.Intersects(_spriteRenderer.bounds);
        if (isUnderPlayer == _isUnderPlayer)
            return;

        _isUnderPlayer = isUnderPlayer;
        if (_isUnderPlayer)
            _fadeTween.Pause();
        else
            _fadeTween.Play();
    }
}
