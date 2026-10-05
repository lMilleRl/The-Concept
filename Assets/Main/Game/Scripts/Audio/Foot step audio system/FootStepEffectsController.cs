using System.Collections.Generic;
using UnityEngine;

public class FootStepEffectsController : IStepEffectsProfileController
{
    private Transform _stepsSource;
    private ISurfaceDetector _surfaceDetector;
    private IStepEventSource _stepEventSource;
    private Transform _feetPoint;

    private Vector2 _prevStepsSourcePos;
    private Vector2 _lastMovementDirection = Vector2.down;
    private Foot _lastFoot = Foot.Right;

    private bool _isSourceMoving;

    private Dictionary<StepEffectsProfileType, StepEffectsProfileData> _profilesData;
    private StepEffectsProfileData _activeProfileData;

    public FootStepEffectsController(FootStepDependencies dependencies, StepEffectsProfileData[] profilesData)
    {
        _stepsSource = dependencies.StepsSource;
        _surfaceDetector = dependencies.SurfaceDetector;
        _stepEventSource = dependencies.StepEventSource;
        _feetPoint = dependencies.FeetPoint != null ? dependencies.FeetPoint : _stepsSource;

        _prevStepsSourcePos = _stepsSource.position;

        _profilesData = new Dictionary<StepEffectsProfileType, StepEffectsProfileData>();
        foreach (var p in profilesData)
        {
            _profilesData.TryAdd(p.ProfileType, p);
        }

        if (_stepEventSource != null)
            _stepEventSource.StepPerformed += OnStepPerformed;
        else
            Debug.LogWarning($"{nameof(FootStepEffectsController)}: {nameof(IStepEventSource)} is not set, animation step events will be ignored");
    }

    public void Dispose()
    {
        if (_stepEventSource != null)
            _stepEventSource.StepPerformed -= OnStepPerformed;
    }

    public void Tick()
    {
        if (!_isSourceMoving) return;

        var movementDelta = (Vector2)_stepsSource.position - _prevStepsSourcePos;
        if (movementDelta.sqrMagnitude > 1e-8f)
            _lastMovementDirection = movementDelta.normalized;

        _prevStepsSourcePos = _stepsSource.position;
    }

    public void SetMovementActive(bool isActive)
    {
        bool wasMoving = _isSourceMoving;
        _isSourceMoving = isActive;

        if (isActive)
        {
            _prevStepsSourcePos = _stepsSource.position;
        }
        else if (wasMoving)
        {
            ExecuteStep(Opposite(_lastFoot), isStop: true);
        }
    }

    public void ResetMovement()
    {
        _isSourceMoving = false;
    }

    private void OnStepPerformed(StepEvent stepEvent)
    {
        if (!_isSourceMoving) return;

        ExecuteStep(stepEvent.Foot, false);
        _lastFoot = stepEvent.Foot;
    }

    private void ExecuteStep(Foot foot, bool isStop)
    {
        if (_activeProfileData.StepEffectStrategies == null)
            return;

        var position = _feetPoint.position;
        var context = new StepEffectContext(
            _surfaceDetector.GetSurface(position),
            position,
            _lastMovementDirection,
            foot,
            isStop);

        foreach (var s in _activeProfileData.StepEffectStrategies)
            s.Execute(context);
    }

    private static Foot Opposite(Foot foot)
    {
        return foot == Foot.Left ? Foot.Right : Foot.Left;
    }

    public void SetProfile(StepEffectsProfileType profileType)
    {
        _activeProfileData = _profilesData[profileType];
    }
}
