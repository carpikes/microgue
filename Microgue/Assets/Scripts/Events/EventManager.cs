using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class EventManager : MonoBehaviour {

    class BundleUnityEvent : UnityEvent<Bundle>
    {

    }

	[Header("If set the manager is kept across scenes")]
	public bool m_DontDestroyOnLoad = true;
	private Dictionary <Events, UnityEvent<Bundle>> eventDictionary;

	private static EventManager eventManager;

	public static EventManager instance
	{
		get
		{
			if (!eventManager)
			{
				eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

				if (!eventManager) 
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManager.Init (); 
				}
			}

			return eventManager;
		}
	}

	void Init ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<Events, UnityEvent<Bundle>>();
			if (m_DontDestroyOnLoad) DontDestroyOnLoad (gameObject);
		}
	}

	public static void StartListening (Events eventName, UnityAction<Bundle> listener)
	{
		UnityEvent<Bundle> thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new BundleUnityEvent();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}

	public static void StopListening (Events eventName, UnityAction<Bundle> listener)
	{
		if (eventManager == null) return;
		UnityEvent<Bundle> thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void TriggerEvent (Events eventName, Bundle eventData)
	{
		UnityEvent<Bundle> thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke ( eventData );
		}
	}
}