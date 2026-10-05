using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FootprintData", menuName = "Game/Footprint Data")]
public class FootprintData : ScriptableObject
{
    [SerializeField] private FootprintSurfaceData[] _footprintSurfaceData;
    [Tooltip("Куда смотрит носок на спрайте (градусы, 90 = вверх)")]
    [SerializeField] private float _spriteForwardAngle = 90f;
    [Tooltip("Боковое смещение каждого следа от центра, в юнитах")]
    [SerializeField] private float _feetSpacing = 0.125f;

    private Dictionary<SurfaceType, FootprintSurfaceData> _footprintSurfaceDictionary;

    public float SpriteForwardAngle => _spriteForwardAngle;
    public float FeetSpacing => _feetSpacing;

    private void OnValidate()
    {
        BuildDictionary();
    }

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        _footprintSurfaceDictionary = new Dictionary<SurfaceType, FootprintSurfaceData>();

        if (_footprintSurfaceData == null)
            return;

        foreach (var footprintSurfaceData in _footprintSurfaceData)
            _footprintSurfaceDictionary.TryAdd(footprintSurfaceData.SurfaceTypeToLink, footprintSurfaceData);
    }

    public bool TryGetFootprintSprite(SurfaceType surfaceType, Foot foot, out Sprite sprite, out bool isMirrored)
    {
        sprite = null;
        isMirrored = false;

        if (_footprintSurfaceDictionary == null)
            BuildDictionary();

        if (!_footprintSurfaceDictionary.TryGetValue(surfaceType, out var data))
            return false;

        if (foot == Foot.Left && data.LeftFootSprites != null && data.LeftFootSprites.Length > 0)
        {
            sprite = data.LeftFootSprites[Random.Range(0, data.LeftFootSprites.Length)];
            return true;
        }

        if (data.RightFootSprites == null || data.RightFootSprites.Length == 0)
        {
            Debug.LogWarning($"No footprint sprites configured for surface type: {surfaceType}", this);
            return false;
        }

        sprite = data.RightFootSprites[Random.Range(0, data.RightFootSprites.Length)];
        isMirrored = foot == Foot.Left;
        return true;
    }
}

[System.Serializable]
public struct FootprintSurfaceData
{
    [FormerlySerializedAs("FootprintSprites")]
    public Sprite[] RightFootSprites;
    [Tooltip("Если пусто — используются зеркальные спрайты правой ноги")]
    public Sprite[] LeftFootSprites;
    public SurfaceType SurfaceTypeToLink;
}
