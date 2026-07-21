using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Wind Signal")]
    [SerializeField] private MonoBehaviour _windSignalSource;

    [Header("Wind Parameters")]
    [SerializeField] private Vector2 _windDirection = Vector2.right;
    [SerializeField] private float _maxStrength = 1.5f;

    [Header("Wave Speed")]
    [SerializeField] private float _minimumWaveSpeedMultiplier = 0.6f;
    [SerializeField] private float _maximumWaveSpeedMultiplier = 1.4f;

    public float WindStrength { get; private set; }
    public Vector2 WindDirection { get; private set; }

    private static readonly int GlobalWindStrengthId = Shader.PropertyToID("_GlobalWindStrength");
    private static readonly int GlobalWindDirectionId = Shader.PropertyToID("_GlobalWindDirection");
    private static readonly int GlobalWindTimeId = Shader.PropertyToID("_GlobalWindTime");

    private IWindSignal _windSignal;
    private float _windTime;

    private void Awake()
    {
        _windSignal = _windSignalSource as IWindSignal;

        if (_windSignalSource != null && _windSignal == null)
            Debug.LogError($"{nameof(WindController)} requires an {nameof(IWindSignal)} source.", this);

        WindDirection = _windDirection.sqrMagnitude > 0f ? _windDirection.normalized : Vector2.right;
        Shader.SetGlobalVector(GlobalWindDirectionId, WindDirection);
    }

    private void Update()
    {
        float normalizedStrength = _windSignal?.NormalizedStrength ?? 0f;

        WindStrength = Mathf.Clamp01(normalizedStrength) * _maxStrength;
        float waveSpeedMultiplier = Mathf.Lerp(_minimumWaveSpeedMultiplier, _maximumWaveSpeedMultiplier,
            Mathf.Clamp01(normalizedStrength));

        _windTime += Time.deltaTime * waveSpeedMultiplier;

        Shader.SetGlobalFloat(GlobalWindStrengthId, WindStrength);
        Shader.SetGlobalFloat(GlobalWindTimeId, _windTime);
    }
}