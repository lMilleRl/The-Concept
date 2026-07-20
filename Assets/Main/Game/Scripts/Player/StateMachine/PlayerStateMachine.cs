using UnityEngine;

[RequireComponent(typeof(PlayerBootstrapStateMachine))]
public class PlayerStateMachine : MonoBehaviour
{
    private IStateMachine _coreStateMachine;

    public void Init(IStateMachine initCoreStateMachine)
    {
        _coreStateMachine = initCoreStateMachine;
    }

    private void Update()
    {
        _coreStateMachine.Update();
    }
    
    private void FixedUpdate()
    {
        _coreStateMachine.FixedUpdate();
    }
}
