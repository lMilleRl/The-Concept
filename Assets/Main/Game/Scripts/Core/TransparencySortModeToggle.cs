using System;
using UnityEngine.Rendering;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransparencySortModeToggle : MonoBehaviour
{
    private void Awake()
    {
        var camera = GetComponent<Camera>();
        camera.transparencySortMode = TransparencySortMode.CustomAxis;
        camera.transparencySortAxis = new Vector3(0f, 1f, 0f);
    }
}
