using System;
using System.Collections.Generic;

public abstract class State
{
    private StateMachine _stateMachine;

    internal void Bind(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    protected void ChangeState<TState>()
        where TState : State
    {
        _stateMachine.ChangeState<TState>();
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class StateMachine
{
    private readonly Dictionary<Type, State> _states = new();
    private State _currentState;

    public State CurrentState => _currentState;

    public void Register<TState>(TState state)
        where TState : State
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        _states.Add(typeof(TState), state);
        state.Bind(this);
    }

    public void ChangeState<TState>()
        where TState : State
    {
        TState nextState = GetState<TState>();

        if (ReferenceEquals(_currentState, nextState))
            return;

        _currentState?.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }
    // public void Update()
    // {
    //     _currentState?.Update();
    // }
    public void Dispose()
    {
        _currentState?.Exit();
        _currentState = null;
    }

    private TState GetState<TState>()
        where TState : State
    {
        if (_states.TryGetValue(typeof(TState), out State state))
            return (TState)state;

        throw new InvalidOperationException(
            $"State {typeof(TState).Name} is not registered.");
    }
}
