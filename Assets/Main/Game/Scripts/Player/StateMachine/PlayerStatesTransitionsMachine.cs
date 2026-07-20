using UnityEngine;

[RequireComponent(typeof(PlayerBootstrapStateMachine))]
public class PlayerStatesTransitionsMachine : MonoBehaviour
{
    private ITransitionMachine _coreMachine;

    public void Init(ITransitionMachine initCoreMachine)
    {
        _coreMachine = initCoreMachine;
    }

    private void Update()
    {
        _coreMachine.UpdateTransitions();
    }
}
