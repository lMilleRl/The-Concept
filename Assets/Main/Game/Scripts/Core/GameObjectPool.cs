using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : MonoBehaviour, IPoolable<T>
{
    private readonly Queue<T> _pooledObjects;
    private readonly T _prefab;

    public GameObjectPool(T prefab)
    {
        _pooledObjects = new Queue<T>();
        _prefab = prefab;
    }

    public T Get()
    {
        if (_pooledObjects.Count == 0)
            InstantiateNewItem();

        var obj = _pooledObjects.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    private void InstantiateNewItem()
    {
        var obj = Object.Instantiate(_prefab);
        obj.gameObject.SetActive(false);
        obj.InitForPool(ReturnToPool);
        _pooledObjects.Enqueue(obj);
    }

    private void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _pooledObjects.Enqueue(obj);
    }
}
