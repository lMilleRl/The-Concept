using System;
using DG.Tweening;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease = Ease.Linear;

    private void OnEnable()
    {
        
    }

    public void MoveAToB()
    {
        transform.position = _pointA.position;
        transform.DOMove(_pointB.position, _duration).SetEase(_ease);
    }
}
