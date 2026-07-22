using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    [SerializeField] private Transform _attachmentPlayerEnterPoint;
    [SerializeField] private Transform _attachmentPlayerExitPoint;
    [SerializeField] private SpriteRenderer _ladderSpriteRenderer;

    public Transform AttachmentPlayerEnterPoint => _attachmentPlayerEnterPoint;
    public Transform AttachmentPlayerExitPoint => _attachmentPlayerExitPoint;
    public SpriteRenderer LadderSpriteRenderer => _ladderSpriteRenderer;
}