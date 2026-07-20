using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Wind Parameters")]
    [SerializeField] private float _maxStrength = 1.5f;
    [SerializeField] private float _strengthChangeSpeed = 0.1f;
    [SerializeField] private float _directionChangeSpeed = 0.05f;

    [Header("Shader Property Names")]
    [SerializeField] private string _windStrengthProperty = "_WindStrength";
    [SerializeField] private string _windDirectionProperty = "_WindDirection";

    public float WindStrength { get; private set; }
    public float WindDirection { get; private set; }

    private int _windStrengthID;
    private int _windDirectionID;

    private void Awake()
    {
        _windStrengthID = Shader.PropertyToID(_windStrengthProperty);
        _windDirectionID = Shader.PropertyToID(_windDirectionProperty);
    }

    private void Update()
    {
        WindStrength = Mathf.PerlinNoise(Time.time * _strengthChangeSpeed, 0f) * _maxStrength;
        WindDirection = Mathf.PerlinNoise(0f, Time.time * _directionChangeSpeed) > 0.5f ? 1f : -1f;

        Shader.SetGlobalFloat(_windStrengthID, WindStrength);
        Shader.SetGlobalFloat(_windDirectionID, WindDirection);
    }
}
