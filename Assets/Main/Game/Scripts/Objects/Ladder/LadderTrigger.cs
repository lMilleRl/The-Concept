using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    [SerializeField] private Transform _attachmentPlayerEnterPoint;
    [SerializeField] private Transform _attachmentPlayerExitPoint;

    public Transform AttachmentPlayerEnterPoint => _attachmentPlayerEnterPoint;
    public Transform AttachmentPlayerExitPoint => _attachmentPlayerExitPoint;
}
