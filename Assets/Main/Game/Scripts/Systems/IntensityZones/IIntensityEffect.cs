using UnityEngine;

public interface IIntensityEffect
{
    void Initialize();
    void Apply(float intensity);
    void OnDisabled(float releaseSpeed);
}
