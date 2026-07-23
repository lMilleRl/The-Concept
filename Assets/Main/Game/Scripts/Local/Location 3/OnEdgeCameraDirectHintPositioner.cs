using Cinemachine;
using UnityEngine;

public class OnEdgeCameraDirectHintPositioner : MonoBehaviour
{
    private const float SegmentParameterTolerance = 0.0001f;
    private const float ParallelTolerance = 0.000001f;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _targetToDirectHint;
    [SerializeField] private Transform _hint;

    private readonly Vector2[] _cameraCorners = new Vector2[4];

    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(HandleCameraUpdated);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(HandleCameraUpdated);
    }

    private void HandleCameraUpdated(CinemachineBrain brain)
    {
        if (brain.OutputCamera != _camera)
            return;

        Vector2 hintPosition = CalculateLightPosition();
        _hint.position = new Vector3(hintPosition.x, hintPosition.y, _hint.position.z);
    }

    private Vector2 CalculateLightPosition()
    {
        UpdateCameraCornersInWorldSpace();
        Vector2 targetPosition = _targetToDirectHint.position;

        if (IsPointInsideCameraBounds(targetPosition))
            return targetPosition;

        return FindClosestCameraBoundsIntersection(targetPosition);
    }

    private void UpdateCameraCornersInWorldSpace()
    {
        float distanceToTargetPlane = _targetToDirectHint.position.z - _camera.transform.position.z;

        _cameraCorners[0] = _camera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceToTargetPlane));
        _cameraCorners[1] = _camera.ViewportToWorldPoint(new Vector3(0f, 1f, distanceToTargetPlane));
        _cameraCorners[2] = _camera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceToTargetPlane));
        _cameraCorners[3] = _camera.ViewportToWorldPoint(new Vector3(1f, 0f, distanceToTargetPlane));
    }

    private bool IsPointInsideCameraBounds(Vector2 point)
    {
        Vector2 bottomLeft = _cameraCorners[0];
        Vector2 topRight = _cameraCorners[2];

        return point.x >= bottomLeft.x
               && point.x <= topRight.x
               && point.y >= bottomLeft.y
               && point.y <= topRight.y;
    }

    private Vector2 FindClosestCameraBoundsIntersection(Vector2 lineEnd)
    {
        Vector2 lineStart = (_cameraCorners[0] + _cameraCorners[2]) * 0.5f;
        Vector2 closestIntersection = lineStart;
        float closestDistanceToDevice = float.PositiveInfinity;

        for (int i = 0; i < _cameraCorners.Length; i++)
        {
            Vector2 sideStart = _cameraCorners[i];
            Vector2 sideEnd = _cameraCorners[(i + 1) % _cameraCorners.Length];

            if (!TryGetLineSegmentsIntersection(
                    lineStart,
                    lineEnd,
                    sideStart,
                    sideEnd,
                    out Vector2 intersection))
                continue;

            float distanceToDevice = Vector2.SqrMagnitude(lineEnd - intersection);
            if (distanceToDevice < closestDistanceToDevice)
            {
                closestDistanceToDevice = distanceToDevice;
                closestIntersection = intersection;
            }
        }

        return closestIntersection;
    }

    private bool TryGetLineSegmentsIntersection(
        Vector2 targetLineStart,
        Vector2 targetLineEnd,
        Vector2 segmentLineStart,
        Vector2 segmentLineEnd,
        out Vector2 intersection)
    {
        intersection = default;

        Vector2 targetDirection = targetLineEnd - targetLineStart;
        Vector2 segmentDirection = segmentLineEnd - segmentLineStart;
        float denominator = Cross(targetDirection, segmentDirection);
        float denominatorScale = Mathf.Sqrt(targetDirection.sqrMagnitude * segmentDirection.sqrMagnitude);

        if (Mathf.Abs(denominator) <= ParallelTolerance * denominatorScale)
            return false;

        Vector2 startOffset = segmentLineStart - targetLineStart;
        float targetParameter = Cross(startOffset, segmentDirection) / denominator;
        float segmentParameter = Cross(startOffset, targetDirection) / denominator;

        if (!IsSegmentParameter(targetParameter) || !IsSegmentParameter(segmentParameter))
            return false;

        intersection = targetLineStart + targetParameter * targetDirection;
        return true;
    }

    private bool IsSegmentParameter(float parameter)
    {
        return parameter >= -SegmentParameterTolerance
               && parameter <= 1f + SegmentParameterTolerance;
    }

    private float Cross(Vector2 first, Vector2 second)
    {
        return first.x * second.y - first.y * second.x;
    }
}