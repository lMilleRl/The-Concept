using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootstepAudioData", menuName = "Game/Footstep Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    [SerializeField]
    private AudioStepData[] _audioStepData;

    private Dictionary<SurfaceType, AudioStepData> _audioStepDictionary;

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
        _audioStepDictionary = new Dictionary<SurfaceType, AudioStepData>();

        if (_audioStepData == null)
            return;

        foreach (var a in _audioStepData)
        {
            _audioStepDictionary.TryAdd(a.SurfaceTypeToLink, a);
        }
    }

    public bool TryGetAudioStepData(SurfaceType surfaceType, out AudioStepData data)
    {
        if (_audioStepDictionary == null)
            BuildDictionary();

        return _audioStepDictionary.TryGetValue(surfaceType, out data);
    }
}

[System.Serializable]
public struct AudioStepData
{
    public AudioClip[] StepsAudio;
    public SurfaceType SurfaceTypeToLink;

    [Range(0.5f, 2f)]
    public float MinPitch;

    [Range(0.5f, 2f)]
    public float MaxPitch;
}
