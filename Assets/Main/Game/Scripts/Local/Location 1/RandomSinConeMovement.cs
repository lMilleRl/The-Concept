using System;
using UnityEngine;

public class RandomSinConeMovement : MonoBehaviour
{
    [Header("Frequency of movement")]
    [SerializeField] private float _verticalFrequency;
    [SerializeField] private float _horizontalFrequency;

    [Header("Form of movement")]
    [SerializeField] private Transform _coneVertexStartPoint;
    [SerializeField] private float _coneHeight;
    [SerializeField] private float _coneRadius;

    private float _verticalPeriodRotation;
    private float _horizontalPeriodRotation;

    private void OnValidate()
    {
        _verticalFrequency = Mathf.Abs(_verticalFrequency);
        _horizontalFrequency = Mathf.Abs(_horizontalFrequency);
        _coneHeight = Mathf.Abs(_coneHeight);
        _coneRadius = Mathf.Abs(_coneRadius);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        UpdatePeriodRotation(ref _verticalPeriodRotation, _verticalFrequency);
        UpdatePeriodRotation(ref _horizontalPeriodRotation, _horizontalFrequency);

        float yConeHeightPercentage = GetNormalizedSin(_verticalPeriodRotation);
        var y = yConeHeightPercentage * _coneHeight;

        var x = Mathf.Sin(_horizontalPeriodRotation) * _coneRadius * yConeHeightPercentage;

        transform.position = new Vector3(_coneVertexStartPoint.position.x + x, _coneVertexStartPoint.position.y + y
            , transform.position.z);

        float GetNormalizedSin(float value) => Mathf.Sin(value) * 0.5f + 0.5f;
    }

    private void UpdatePeriodRotation(ref float periodRotation, float frequency)
    {
        periodRotation = Mathf.Repeat(periodRotation + frequency * Time.deltaTime, 2f * Mathf.PI);
    }

    private void OnDrawGizmos()
    {
        var coneCenterBasePoint = _coneVertexStartPoint.position;
        coneCenterBasePoint.y += _coneHeight;
        Gizmos.DrawLine(_coneVertexStartPoint.position, coneCenterBasePoint);

        var coneBaseStartPoint = coneCenterBasePoint;
        var coneBaseEndPoint = coneCenterBasePoint;
        coneBaseStartPoint.x -= _coneRadius;
        coneBaseEndPoint.x += _coneRadius;
        Gizmos.DrawLine(coneBaseStartPoint, coneBaseEndPoint);
    }
}