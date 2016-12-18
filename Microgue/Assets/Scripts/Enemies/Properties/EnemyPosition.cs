using UnityEngine;
using System.Collections;
using System;

public class EnemyPosition : MonoBehaviour 
{
	private Vector2 mIntentPosition;
	private bool mIsEnabled = false;

	// Use this for initialization
	void Start()
	{
		mIntentPosition = transform.position;
	}

	public void SetWorldPosition(Vector2 pos)
	{
		mIntentPosition = pos;
	}

	public Vector2 GetWorldPosition()
	{
		return mIntentPosition;
	}

	public void SetEnabled(bool enabled)
	{
		mIsEnabled = enabled;
	}

	public bool IsEnabled() 
	{ 
		return mIsEnabled;
	}

    internal bool IsOutOfTileMap()
    {
        // REMEMBER THAT Ys are negative!
        return mIntentPosition.x < 0 || mIntentPosition.x >= AIMap.GetWidth() 
            || mIntentPosition.y > 0 || mIntentPosition.y <= -AIMap.GetHeight();
    }
}
