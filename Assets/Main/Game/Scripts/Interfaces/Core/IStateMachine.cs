using UnityEngine;

public interface IStateMachine
{
    IState CurrentState { get; }
    
    void Update();
    void FixedUpdate();
    void SetNextState(IState nextState);
}
