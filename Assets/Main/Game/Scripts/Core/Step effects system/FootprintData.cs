using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootprintData", menuName = "Game/Footprint Data")]
public class FootprintData : ScriptableObject
{
    [SerializeField] private FootprintSurfaceData[] _footprintSurfaceData;

    private Dictionary<SurfaceType, FootprintSurfaceData> _footprintSurfaceDictionary;

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

    public Sprite[] GetFootprintSprites(SurfaceType surfaceType)
    {
        if (_footprintSurfaceDictionary == null)
            BuildDictionary();

        if (_footprintSurfaceDictionary.TryGetValue(surfaceType, out var data))
        {
            if (data.FootprintSprites == null || data.FootprintSprites.Length == 0)
            {
                Debug.LogWarning($"No footprint sprites configured for surface type: {surfaceType}", this);
                return null;
            }

            return data.FootprintSprites;
        }

        Debug.LogWarning($"Surface type {surfaceType} not found in {nameof(FootprintData)}", this);
        return null;
    }
}

[System.Serializable]
public struct FootprintSurfaceData
{
    public Sprite[] FootprintSprites;
    public SurfaceType SurfaceTypeToLink;
}
