using System;
using UnityEngine;

public class Footprint : MonoBehaviour, IPoolable<Footprint>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _lifeTimeInSec;
    
    private Action<Footprint> _returnToPool;

    private void OnEnable()
    {
        CancelInvoke(nameof(Release));
        Invoke(nameof(Release), _lifeTimeInSec);
    }

    public void SetSprite(Sprite sprite, bool isMirrored)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.flipX = isMirrored;
    }

    public void InitForPool(Action<Footprint> returnToPool)
    {
        _returnToPool = returnToPool;
    }

    public void Release()
    {
        _returnToPool?.Invoke(this);
    }
}
