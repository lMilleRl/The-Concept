using System;
using UnityEngine;

[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerStatesTransitionsMachine))]
public class PlayerBootstrapStateMachine : MonoBehaviour
{
    private PlayerStateMachineDependencies _dependencies;

    private PlayerStateMachine _playerStateMachine;
    private PlayerStatesTransitionsMachine _playerStatesTransitionsMachine;

    private IStateMachine _coreStateMachine;
    private ITransitionMachine _coreTransitionMachine;
    private IState[] _states;


    public void Init(PlayerStateMachineDependencies dependencies)
    {
        _playerStateMachine = GetComponent<PlayerStateMachine>();
        _playerStatesTransitionsMachine = GetComponent<PlayerStatesTransitionsMachine>();
        _dependencies = dependencies;
        
        _states = GeneratePlayerStates();
        InitStateMachine(_states);
        InitTransitionMachine();
    }

    private void InitTransitionMachine()
    {
        _coreTransitionMachine = new TransitionMachine(_coreStateMachine, GenerateStatesTransitions());
        
        _playerStatesTransitionsMachine.Init(_coreTransitionMachine);
    }

    private void InitStateMachine(IState[] initStates)
    {
        _coreStateMachine = new StateMachine(initStates[0], initStates);

        _playerStateMachine.Init(_coreStateMachine);
    }

    private StateTransition[] GenerateStatesTransitions()
    {
        var enterClimbingTransitionData = new BaseStateTransitionData(_states[0], _states[2], false);
        var enterClimbingTransition =
            new ClimbingTransition(enterClimbingTransitionData, _dependencies.ClimbingStateData.LadderDetector);

        var exitClimbingTransitionData = new BaseStateTransitionData(_states[2], _states[0], false);
        var exitClimbingTransition =
            new ClimbingTransition(exitClimbingTransitionData, _dependencies.ClimbingStateData.LadderDetector);

        var enterPassiveStateTransData = new BaseStateTransitionData(null, _states[1], true);
        Predicate<GameState> enterCondition = currentState => currentState != GameState.Gameplay;
        var enterPassiveStateTrans =
            new ByGameStateTransition(enterPassiveStateTransData, GameStateManager.Instance, enterCondition);

        var exitPassiveStateTransData = new BaseStateTransitionData(_states[1], null, false);
        var exitPassiveStateTrans = new FromPassiveStateTransition(exitPassiveStateTransData, _coreStateMachine,
            enterPassiveStateTrans, GameStateManager.Instance);

        return new StateTransition[] { enterClimbingTransition, exitClimbingTransition, enterPassiveStateTrans, exitPassiveStateTrans };
    }

    private IState[] GeneratePlayerStates()
    {
        var baseStateData = new GameplayPlayerStateData(_dependencies.Movement, _dependencies.Interaction
            , _dependencies.InteractionInput, _dependencies.PlayerMoveInput);
        var baseState = new GameplayPlayerState(baseStateData);

        var passiveStateData = new GameplayPlayerStateData(_dependencies.Movement, _dependencies.Interaction
            , null, null);
        var passiveState = new GameplayPlayerState(passiveStateData);

        var climbingState = new ClimbingPlayerState(_dependencies.ClimbingStateData);

        return new IState[] { baseState, passiveState, climbingState };
    }
}