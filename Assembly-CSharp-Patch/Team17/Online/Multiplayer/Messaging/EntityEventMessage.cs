using BitStream;
using LevelEditor;

namespace Team17.Online.Multiplayer.Messaging
{
	public class EntityEventMessage : Serialisable
	{
		public EntityMessageHeader m_Header = new EntityMessageHeader();

		public uint m_ComponentId;

		public Serialisable m_Payload;

		public void Initialise(EntityMessageHeader header, uint uComponentId, Serialisable payload)
		{
			m_Header = header;
			m_ComponentId = uComponentId;
			m_Payload = payload;
		}

		public void Serialise(BitStreamWriter writer)
		{
			m_Header.Serialise(writer);
			writer.Write((byte)m_ComponentId, 4);
			m_Payload.Serialise(writer);
		}

		public bool Deserialise(BitStreamReader reader)
		{
			if (m_Header.Deserialise(reader))
			{
				m_ComponentId = reader.ReadByte(4);
				EntitySerialisationEntry entry = EntitySerialisationRegistry.GetEntry(m_Header.m_uEntityID);
				if (entry != null)
				{
					ClientSynchroniser clientSynchroniser = entry.m_ClientSynchronisedComponents._items[m_ComponentId];
					// patch
					IModEntityClient modEntity = clientSynchroniser as IModEntityClient;
					if (modEntity != null)
					{
						m_Payload = modEntity.GetSerialisable();
						if (m_Payload != null)
							return m_Payload.Deserialise(reader);
						else
							return false;
					}
					// patch
					return SerialisationRegistry<EntityType>.Deserialise(out m_Payload, clientSynchroniser.GetEntityType(), reader);
				}
				return false;
			}
			return false;
		}
	}
}
