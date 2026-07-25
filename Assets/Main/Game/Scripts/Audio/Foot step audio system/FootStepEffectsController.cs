using System.Collections.Generic;
using UnityEngine;

public class FootStepEffectsController : IStepEffectsProfileController
{
    private Transform _stepsSource;
    private ISurfaceDetector _surfaceDetector;
    private float _distanceBetweenSteps;
    private Vector2 _prevStepsSourcePos;
    private float _currentDistance;
    private Vector2 _lastMovementDirection;

    private bool _isSourceMoving;
    
    private Dictionary<StepEffectsProfileType, StepEffectsProfileData> _profilesData;
    private StepEffectsProfileData _activeProfile;
    
    public FootStepEffectsController(FootStepDependencies dependencies, StepEffectsProfileData[] profilesData)
    {
        _stepsSource = dependencies.StepsSource;
        _surfaceDetector = dependencies.SurfaceDetector;

        _prevStepsSourcePos = _stepsSource.position;

        _profilesData = new Dictionary<StepEffectsProfileType, StepEffectsProfileData>();
        foreach (var p in profilesData)
        {
            _profilesData.TryAdd(p.ProfileType, p);
        }
    }

    public void Tick()
    {
        if (!_isSourceMoving) return;
        
        var movementDelta = (Vector2)_stepsSource.position - _prevStepsSourcePos;
        if (movementDelta.sqrMagnitude > 0f)
            _lastMovementDirection = movementDelta.normalized;

        if (_currentDistance >= _distanceBetweenSteps)
        {
            ExecuteActiveStrategy();
        }

        _currentDistance += movementDelta.magnitude;
        _prevStepsSourcePos = _stepsSource.position;
    }

    public void SetMovementActive(bool isActive)
    {
        _isSourceMoving = isActive;

        if (_isSourceMoving == false)
        {
            ExecuteActiveStrategy();
        }
    }

    private void ExecuteActiveStrategy()
    {
        var context = new StepEffectContext(
            _surfaceDetector.GetSurface(_stepsSource.position),
            _stepsSource.position,
            _lastMovementDirection);
        foreach (var s in _activeProfile.StepEffectStrategies)
            s.Execute(context);
        _currentDistance = 0f;
    }

    public void SetProfile(StepEffectsProfileType profileType)
    {
        _activeProfile = _profilesData[profileType];
        _distanceBetweenSteps = _profilesData[profileType].DistanceBetweenSteps;
    }
}