using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCallback : MonoBehaviour
{
	public delegate void Callback();

	[SerializeField]
	private string[] m_allowedTriggers;

	private Dictionary<string, Callback> m_callbackRegistry = new Dictionary<string, Callback>();

	public void RegisterCallback(string _trigger, Callback _callback)
	{
		(m_callbackRegistry)[_trigger] = (Callback)Delegate.Combine(m_callbackRegistry[_trigger], _callback);
	}

	public void UnregisterCallback(string _trigger, Callback _callback)
	{
		(m_callbackRegistry)[_trigger] = (Callback)Delegate.Remove(m_callbackRegistry[_trigger], _callback);
	}

	private void Awake()
	{
		for (int i = 0; i < m_allowedTriggers.Length; i++)
		{
			m_callbackRegistry.Add(m_allowedTriggers[i], delegate
			{
			});
		}
	}

	private void OnTrigger(string _trigger)
	{
		if (m_callbackRegistry.ContainsKey(_trigger))
		{
			m_callbackRegistry[_trigger]();
		}
	}
}
