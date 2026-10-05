public readonly struct StepEffectsProfileData
{
    public readonly StepEffectsProfileType ProfileType;
    public readonly IStepEffectStrategy[] StepEffectStrategies;

    public StepEffectsProfileData(StepEffectsProfileType profileType, IStepEffectStrategy[] stepEffectStrategies)
    {
        ProfileType = profileType;
        StepEffectStrategies = stepEffectStrategies;
    }
}
