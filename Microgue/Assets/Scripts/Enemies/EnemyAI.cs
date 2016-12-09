using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour 
{
	private Vector2 mIntentPosition;
	private bool mIsEnabled = false;

	// Use this for initialization
	void Start()
	{
		mIntentPosition = transform.position;
	}

	public void SetPosition(Vector2 pos)
	{
		mIntentPosition = pos;
	}

	public Vector2 GetPosition()
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
}
