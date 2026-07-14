using System;
using UnityEngine;

/// <summary>
/// Clamps active particles inside a 2D rectangle in world space.
/// Works only when the particle system uses World Simulation Space.
/// </summary>
public class ParticleBoundary2D : MonoBehaviour
{
    private enum EdgeType
    {
        Solid,
        Killing
    }
    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private EdgeType _edgeType;
    [SerializeField] private Vector2 _min;
    [SerializeField] private Vector2 _max;

    private ParticleSystem.Particle[] _particles;

    private void Reset()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        SetClampsDotsFromShape();
    }

    private void LateUpdate()
    {
        int max = _particleSystem.main.maxParticles;
        if (_particles == null || _particles.Length < max)
            _particles = new ParticleSystem.Particle[max];

        int count = _particleSystem.GetParticles(_particles);
        for (int i = 0; i < count; i++)
        {
            LimitParticle(ref _particles[i], _edgeType);
        }
        _particleSystem.SetParticles(_particles, count);
    }

    private void LimitParticle(ref ParticleSystem.Particle particle, EdgeType edgeType)
    {
        switch (edgeType)
        {
            case EdgeType.Solid:
                LimitParticlePos(ref particle);
                break;
            case EdgeType.Killing:
                ExtinguishParticle(ref particle);
                break;
        }   
    }

    private void LimitParticlePos(ref ParticleSystem.Particle particle)
    {
        Vector3 pos = particle.position;
        pos.x = Mathf.Clamp(pos.x, _min.x, _max.x);
        pos.y = Mathf.Clamp(pos.y, _min.y, _max.y);
        particle.position = pos;
    }
    
    private void ExtinguishParticle(ref ParticleSystem.Particle particle)
    {
        float remainingLifetimeOutsideBound = 0.4f;
        
        if (!IsInBound(particle.position))
        {
            particle.remainingLifetime = Mathf.Min(particle.remainingLifetime, remainingLifetimeOutsideBound);
        }
    }

    private bool IsInBound(Vector2 pos)
    {
        return pos.x >= _min.x && pos.x <= _max.x && pos.y >= _min.y && pos.y <= _max.y;
    }
    
    [ContextMenu("Set Boundaries Dots In The Center Of The Shape")]
    private void SetClampsDotsInCenterOfShape()
    {
        if (_particleSystem == null) 
            return;
        
        var shapePos = transform.position + _particleSystem.shape.position;
        _min = shapePos;
        _max = shapePos;
    }
    
    [ContextMenu("Set Boundaries Dots In Bound Of The Shape")]
    private void SetClampsDotsFromShape()
    {
        if (_particleSystem == null) 
            return;
        
        var shapePos = transform.position + _particleSystem.shape.position;
        var shapeBound = _particleSystem.shape.scale * 0.5f; 
        _min = shapePos - shapeBound;
        _max = shapePos + shapeBound;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float dotRadius = 0.5f;
        
        SetDot(_min, dotRadius, Color.red, "Min");
        SetDot(_max, dotRadius, Color.green, "Max");
    }

    private void SetDot(Vector2 pos, float radius, Color color, string name)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(pos, radius);
        UnityEditor.Handles.Label(pos, name);
    }
    #endif
}
