using System;
using UnityEngine;

public abstract class State : IState
{
    public abstract void Enter();

    public abstract void Update();

    public abstract void FixedUpdate();

    public abstract void Exit();
}
