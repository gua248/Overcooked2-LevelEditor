using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ServerTriggerZone : ServerSynchroniserBase
{
	private TriggerZone m_triggerZone;

	private TriggerZoneMessage m_data = new TriggerZoneMessage();

	private List<Collider> m_collidersOccupying = new List<Collider>();

	public override EntityType GetEntityType()
	{
		return EntityType.TriggerZone;
	}

	public override void StartSynchronising(Component synchronisedObject)
	{
		base.StartSynchronising(synchronisedObject);
		m_triggerZone = (TriggerZone)synchronisedObject;
	}

	public override void UpdateSynchronising()
	{
		base.UpdateSynchronising();
		if (!m_triggerZone.m_fallPad || m_collidersOccupying.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_collidersOccupying.Count; i++)
		{
			if (!m_collidersOccupying[i].enabled)
			{
				m_collidersOccupying.Remove(m_collidersOccupying[i]);
			}
		}
	}

	private void SyncOccupied()
	{
		m_data.Initialise(IsOccupied());
		SendServerEvent(m_data);
	}

	public bool IsOccupied()
	{
		return m_collidersOccupying.Count != 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_collidersOccupying.Count == 0)
		{
			base.gameObject.SendTrigger(m_triggerZone.m_onOccupationTrigger);
		}
		m_collidersOccupying.Add(other);
        // patch
		SyncOccupied();
        // patch
    }

    private void OnTriggerExit(Collider other)
	{
		m_collidersOccupying.RemoveAll(other.Equals);
		if (m_collidersOccupying.Count == 0)
		{
			base.gameObject.SendTrigger(m_triggerZone.m_onDeoccupationTrigger);
		}
        // patch
        SyncOccupied();
        // patch
    }
}
