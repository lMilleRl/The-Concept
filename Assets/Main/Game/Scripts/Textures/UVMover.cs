using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UVMover : MonoBehaviour
{
    [SerializeField] private Vector2 _scrollSpeed = new Vector2(0.05f, 0.05f);
    private Material _material;

    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            _material = spriteRenderer.material;
        }
    }

    private void Update()
    {
        if (_material != null)
        {
            Vector2 offset = _material.mainTextureOffset;
            offset += _scrollSpeed * Time.deltaTime;
            _material.mainTextureOffset = offset;
        }
    }
}
