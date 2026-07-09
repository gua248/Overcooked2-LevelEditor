using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ServerFireHazardSpawner : ServerSynchroniserBase, ITriggerReceiver
{
	private FireHazardSpawner m_spawner;

	private GridManager m_gridManager;

	private Rigidbody m_Rigidbody;

	private Transform m_targetTransform;

	private FireHazardSpawnerMessage m_data = new FireHazardSpawnerMessage();

	public override EntityType GetEntityType()
	{
		return EntityType.FireHazardSpawner;
	}

	public override void StartSynchronising(Component synchronisedObject)
	{
		base.StartSynchronising(synchronisedObject);
		m_spawner = (FireHazardSpawner)synchronisedObject;
		m_gridManager = GameUtils.GetGridManager(base.transform);
		m_Rigidbody = base.gameObject.RequestComponentUpwardsRecursive<Rigidbody>();
		NetworkUtils.RegisterSpawnablePrefab(base.gameObject, m_spawner.m_hazardPrefab);
	}

	private void Burn()
	{
		GridManager gridManager = GameUtils.GetGridManager(base.transform);
		GridIndex gridLocationFromPos = gridManager.GetGridLocationFromPos(m_Rigidbody.position);
		GameObject gridOccupant = gridManager.GetGridOccupant(gridLocationFromPos);
        
		// patch
		if (gridOccupant != null && gridOccupant.RequestComponent<FireHazard>() == null && gridOccupant.RequestComponent<Flammable>() != null)
		{
			ServerFlammable serverFlammable = gridOccupant.RequestComponent<ServerFlammable>();
			serverFlammable.Ignite();
		}
        // patch
        
		if (gridOccupant != null && (bool)gridOccupant.RequestComponent<FireHazard>())
		{
			ServerFireHazard serverFireHazard = gridOccupant.RequestComponent<ServerFireHazard>();
			serverFireHazard.Conflagrate();
		}
		else
		{
			if (!(gridOccupant == null) && !(gridOccupant.RequestInterface<HazardBase>() != null))
			{
				return;
			}
			if ((bool)m_targetTransform)
			{
				IParentable parentable = m_targetTransform.gameObject.RequestInterfaceUpwardsRecursive<IParentable>();
				Transform attachPoint = parentable.GetAttachPoint(base.gameObject);
				if ((bool)attachPoint)
				{
					GameObject gameObject = Object.Instantiate(m_spawner.m_hazardPrefab, attachPoint.position, attachPoint.rotation, attachPoint);
					EntitySerialisationRegistry.ServerRegisterObject(gameObject);
					ComponentCacheRegistry.UpdateObject(gameObject);
					EntitySerialisationEntry entry = EntitySerialisationRegistry.GetEntry(((MonoBehaviour)parentable).gameObject);
					EntitySerialisationEntry entry2 = EntitySerialisationRegistry.GetEntry(gameObject);
					m_data.m_parentEntry = entry;
					m_data.m_spawnedEntry = entry2;
					SendServerEvent(m_data);
				}
			}
			else
			{
				GameObject gameObject2 = NetworkUtils.ServerSpawnPrefab(base.gameObject, m_spawner.m_hazardPrefab);
				if (m_spawner.m_alignToGrid)
				{
					gameObject2.transform.position = gridManager.GetPosFromGridLocation(gridLocationFromPos);
				}
			}
		}
	}

	public void SetTargetTransformToAttach(Transform transform)
	{
		m_targetTransform = transform;
	}

	public void OnTrigger(string _message)
	{
		if (m_spawner.m_spawnTrigger == _message)
		{
			Burn();
		}
	}
}
