using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM 
{
    public enum Step
    {
        Enter, 
        Update, 
        Exit
    }

    public delegate void State(FSM fsm, Step step, State state);

    State currentState;

    public void OnSpawn(State startState)
    {
        TransitionTo(startState);
    }

    public void OnUpdate()
    {
        currentState.Invoke(this, Step.Update, null);
    }

    public void TransitionTo(State state)
    {
        if (currentState != null)
        {
        currentState.Invoke(this, Step.Exit, state);
        }
        var oldState = currentState;
        currentState = state;
        currentState.Invoke(this, Step.Enter, oldState);
    }
}
