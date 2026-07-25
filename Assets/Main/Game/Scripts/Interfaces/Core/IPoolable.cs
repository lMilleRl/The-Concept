using System;

public interface IPoolable<T>
{
    void InitForPool(Action<T> returnToPool);
    void Release();
}
