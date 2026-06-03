using System.Collections.Generic;
using UnityEngine;

namespace Team17.Online.Multiplayer.Messaging
{
	public class SpawnableEntityCollection : MonoBehaviour, INetworkEntitySpawner
	{
		[SerializeField]
		[ReadOnly]
		private List<GameObject> m_spawnables = new List<GameObject>();

		private Dictionary<int, List<VoidGeneric<GameObject>>> m_callbacks = new Dictionary<int, List<VoidGeneric<GameObject>>>();

		public void RegisterSpawnable(GameObject _prefab, VoidGeneric<GameObject> _spawnCallback)
		{
			if (!m_spawnables.Contains(_prefab))
			{
				m_spawnables.Add(_prefab);
				List<VoidGeneric<GameObject>> list = new List<VoidGeneric<GameObject>>();
				list.Add(_spawnCallback);
				m_callbacks.Add(GetSpawnableID(_prefab), list);
			}
			else
			{
				List<VoidGeneric<GameObject>> list2 = m_callbacks[GetSpawnableID(_prefab)];
				if (!list2.Contains(_spawnCallback))
				{
					list2.Add(_spawnCallback);
				}
			}
		}

		public GameObject SpawnEntity(int _id, Vector3 _position, Quaternion _rotation, ref List<VoidGeneric<GameObject>> callbacks)
		{
			if (m_spawnables != null && m_spawnables.Count > 0)
			{
				GameObject source = m_spawnables[_id];
				GameObject gameObject = source.Instantiate(_position, _rotation);
				// patch
				gameObject.SetActive(true);
				// patch
				gameObject.transform.SetParent(null, true);
				callbacks = m_callbacks[_id];
				return gameObject;
			}
			return null;
		}

		public int GetSpawnableID(GameObject _object)
		{
			int result = -1;
			if (m_spawnables != null && m_spawnables.Count > 0)
			{
				result = m_spawnables.IndexOf(_object);
			}
			return result;
		}

		public GameObject AccessGameObject()
		{
			return base.gameObject;
		}
	}
}
