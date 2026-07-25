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

    public AudioClip[] GetFootStepAudio(SurfaceType surfaceType)
    {
        if (_audioStepDictionary == null)
            BuildDictionary();

        if (_audioStepDictionary.TryGetValue(surfaceType, out var data))
        {
            if (data.StepsAudio == null || data.StepsAudio.Length == 0)
            {
                Debug.LogWarning($"No audio clips configured for surface type: {surfaceType}", this);
                return null;
            }

            return data.StepsAudio;
        }

        Debug.LogWarning($"Surface type {surfaceType} not found in {nameof(FootStepAudioData)}", this);
        return null;
    }
}

[System.Serializable]
public struct AudioStepData
{
    public AudioClip[] StepsAudio;
    public SurfaceType SurfaceTypeToLink;
}
