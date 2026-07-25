using System;
using UnityEngine;

public class FootStepControllerHandler : MonoBehaviour, IToggleable, IMovementStateListener, IStepEffectsProfileController
{
    [SerializeField] private Transform _stepsSource;
    [SerializeField] private MonoBehaviour _surfaceDetectorSource;
    [SerializeField] private FootStepAudioData _audioData;
    [SerializeField] private AudioSource _soundsPlayer;

    private FootStepEffectsController _footStepEffectsController;

    public void Init(StepEffectsProfileData[] profiles)
    {
        var dependencies = new FootStepDependencies(
            _stepsSource,
            _surfaceDetectorSource as ISurfaceDetector,
            _audioData,
            _soundsPlayer);

        _footStepEffectsController = new FootStepEffectsController(dependencies, profiles);
    }

    private void Update()
    {
        _footStepEffectsController.Tick();
    }

    public void SetEnabled(bool isEnabled)
    {
        enabled = isEnabled;
    }

    public void SetMovementActive(bool isActive)
    {
        _footStepEffectsController.SetMovementActive(isActive);
    }

    public void SetProfile(StepEffectsProfileType profileType)
    {
        _footStepEffectsController.SetProfile(profileType);
    }
}
