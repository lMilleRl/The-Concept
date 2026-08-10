using UnityEngine;

[CreateAssetMenu(fileName = "New CutsceneMovementCommand", menuName = "Game/Cutscene/Movement Command")]
public class CutsceneMovementCommand : ScriptableObject
{
    [SerializeField] private Vector2 _direction;
    [Range(0f, float.MaxValue)] [SerializeField] private float _durationInSec;
    [Range(0f, 1f)] [SerializeField] private float _inputMagnitude = 1f;

    public Vector2 Direction => _direction;
    public float DurationInSec => _durationInSec;
    public float InputMagnitude => _inputMagnitude;
}
