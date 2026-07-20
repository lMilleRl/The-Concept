using System;
using UnityEngine;

public interface IStateTransition
{
    event Action<IState> ConditionCompleted;
    
    bool TryToEnterState();
}
