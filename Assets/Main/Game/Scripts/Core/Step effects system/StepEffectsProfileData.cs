public readonly struct StepEffectsProfileData
{
    public readonly float DistanceBetweenSteps;
    public readonly StepEffectsProfileType ProfileType;
    public readonly IStepEffectStrategy[] StepEffectStrategies;

    public StepEffectsProfileData(StepEffectsProfileType profileType, IStepEffectStrategy[] stepEffectStrategies, float distanceBetweenSteps)
    {
        ProfileType = profileType;
        StepEffectStrategies = stepEffectStrategies;
        DistanceBetweenSteps = distanceBetweenSteps;
    }
}
