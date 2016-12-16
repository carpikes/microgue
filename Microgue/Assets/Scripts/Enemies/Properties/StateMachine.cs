using UnityEngine;
using System.Collections;

public class StateMachine<Enemy> {

    Enemy mOwner;
    State<Enemy> mCurrentState;
    State<Enemy> mGlobalState;
    State<Enemy> mPreviousState;

    public StateMachine(Enemy owner, State<Enemy> currentState, State<Enemy> globalState) {
        mOwner = owner;

        ChangeState(currentState);
        mPreviousState = null;
        mGlobalState = globalState;
    }

    public void Update()
    {
        if (mCurrentState != null) mCurrentState.Update(mOwner);
        if (mGlobalState != null) mGlobalState.Update(mOwner);
    }

    public void FixedUpdate()
    {
        if (mCurrentState != null) mCurrentState.FixedUpdate(mOwner);
        if (mGlobalState != null) mGlobalState.FixedUpdate(mOwner);
    }

    public void ChangeState( State<Enemy> newState )
    {
        mPreviousState = mCurrentState;
        if( mCurrentState != null ) mCurrentState.OnExit(mOwner);
        mCurrentState = newState;
        mCurrentState.OnEnter(mOwner);
    }

    public void RevertToPreviousState()
    {
        ChangeState(mPreviousState);
    }

    public bool IsCurrentState( State<Enemy> state ) { return state == CurrentState; }

    public State<Enemy> CurrentState
    {
        get
        {
            return mCurrentState;
        }
    }

    public State<Enemy> GlobalState
    {
        get
        {
            return mGlobalState;
        }
    }

    public State<Enemy> PreviousState
    {
        get
        {
            return mPreviousState;
        }
    }
}
