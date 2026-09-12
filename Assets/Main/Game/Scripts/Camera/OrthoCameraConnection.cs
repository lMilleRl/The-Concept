using System;
using UnityEngine;

public class OrthoCameraConnection : MonoBehaviour
{
    [SerializeField] private Camera _cameraToCopyOrtho;
    [SerializeField] private Camera _cameraToPasteOrtho;

    private void Update()
    {
        _cameraToPasteOrtho.orthographicSize = _cameraToCopyOrtho.orthographicSize;
    }
}
