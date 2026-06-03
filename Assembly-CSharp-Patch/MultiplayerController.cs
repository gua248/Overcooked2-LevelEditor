using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameModes;
using GameModes.Horde;
using LevelEditor;
using Team17.Online;
using Team17.Online.Multiplayer;
using Team17.Online.Multiplayer.Connection;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerController : Manager
{
	public enum NodeType
	{
		Uninitialised = 0,
		Server = 1,
		Client = 2
	}

	public const int kBitsPerEntityID = 10;

	private Server m_LocalServer = new Server();

	private Client m_LocalClient = new Client();

	private ServerSynchronisationScheduler m_ServerSync = new ServerSynchronisationScheduler();

	private ClientSynchronisationReceiver m_ClientSync = new ClientSynchronisationReceiver();

	private EntitySerialisationRegistry m_EntitySerialiser = new EntitySerialisationRegistry();

	private InviteMonitor m_InviteMonitor = new InviteMonitor();

	private NodeType m_NodeType;

	private ConnectionStatus m_ConnectionStatus = new ConnectionStatus();

	private DisconnectionHandler m_DisconnectionHandler = new DisconnectionHandler();

	private static bool m_bSyncActive;

	private IEnumerator m_scanCoroutine;

	public NetworkPredictionTweekables m_NetworkPredictionTweekables;

	public bool ScanActive
	{
		get
		{
			return m_scanCoroutine != null;
		}
	}

	private void Awake()
	{
		SerialisationRegistry<MessageType>.Initialise(new MessageTypeComparer());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.EntitySynchronisation, new EntitySynchronisationMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.EntityEvent, new EntityEventMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.SpawnEntity, new SpawnEntityMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.DestroyEntity, new DestroyEntityMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.Input, new ControllerStateMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ChefOwnership, new UsersChangedMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.UsersChanged, new UsersChangedMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.UsersAdded, new UserAddedMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.LevelLoadByIndex, new LevelLoadByIndexMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.LevelLoadByName, new LevelLoadByNameMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.GameState, new GameStateMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ChefAvatar, new ChefAvatarMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.LobbyServer, new LobbyServerMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.LobbyClient, new LobbyClientMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.DynamicLevel, new DynamicLevelMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ChefEvent, new ChefEventMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ChefEffect, new ChefEffectMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.LatencyMeasure, new LatencyMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.TimeSync, new TimeSyncMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ControllerSettings, new ControllerSettingsMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.MapAvatar, new MapAvatarControlsMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.MapAvatarHorn, new MapAvatarHornMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.GameSetup, new GameSetupMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.GameProgressData, new GameProgressDataNetworkMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.EmoteWheel, new EmoteWheelMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.SetupCoopSession, new SetupCoopSessionNetworkMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.Achievement, new AchievementMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.TriggerAudio, new TriggerAudioMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.SpawnPhysicalAttachment, new SpawnPhysicalAttachmentMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ResumeWorldObjectSync, new ResumeObjectSyncMessage<WorldObjectMessage>());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ResumeChefPositionSync, new ResumeObjectSyncMessage<ChefPositionMessage>());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ResumePhysicsObjectSync, new ResumeObjectSyncMessage<PhysicsObjectMessage>());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.BossLevel, new BossLevelMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.DestroyChef, new DestroyChefMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.HighScores, new HighScoresMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.DestroyEntities, new DestroyEntitiesMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.ResumeEntitySync, new ResumeEntitySyncMessage());
		SerialisationRegistry<MessageType>.RegisterMessageType(MessageType.SessionConfigSync, new SessionConfigSyncMessage());
		SerialisationRegistry<EntityType>.Initialise(new EntityTypeComparer());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.WorldObject, new WorldObjectMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.WorldPopup, new WorldMapInfoPopupMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PhysicsObject, new PhysicsObjectMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Chef, new ChefPositionMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.SprayingUtensil, new SprayingUtensilMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.RespawnBehaviour, new RespawnMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Flammable, new FlammableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Workstation, new WorkstationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Workable, new WorkableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PlateStation, new PlateStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PlateStack, new PlateStackMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.WashingStation, new WashingStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PhysicalAttach, new PhysicalAttachMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.IngredientContainer, new IngredientContainerMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.CookingState, new CookingStateMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.MixingState, new MixingStateMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.AttachStation, new AttachStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.CookingStation, new CookingStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.ConveyorStation, new ConveyorStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.ConveyorAnimator, new ConveyorAnimationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TimedQueue, new TimedQueueMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerZone, new TriggerZoneMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerDisable, new TriggerDisableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerOnAnimator, new TriggerOnAnimatorMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerToggleOnAnimator, new TriggerToggleOnAnimatorMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerMoveSpawn, new MoveSpawnMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerDialogue, new TriggerDialogueMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.AnimatorVariable, new TriggerAnimatorVariableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.ChefCarry, new ChefCarryMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.InputEvent, new InputEventMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.FlowController, new KitchenFlowMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.ThrowableItem, new ThrowableItemMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Teleportal, new TeleportalMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Cutscene, new CutsceneStateMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Dialogue, new DialogueStateMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TutorialPopup, new TutorialDismissMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.AttachCatcher, new AttachmentCatcherMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.SessionInteractable, new SessionInteractableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.WorldMapVanControls, new MapAvatarControlsMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.WorldMapVanAvatar, new AvatarPositionMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.SwitchMapNode, new SwitchMapNodeMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.LevelPortalMapNode, new LevelPortalMapNodeMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.RubbishBin, new RubbishBinMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.MixingStation, new MixingStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Washable, new WashableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.HeatedStation, new HeatedStationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.RespawnCollider, new RespawnColliderMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.AutoWorkstation, new AutoWorkstationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PlacementItemSpawner, new PlacementItemSpawnerMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.HordeFlowController, default(HordeFlowMessage));
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.HordeTarget, default(HordeTargetMessage));
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.HordeEnemy, default(HordeEnemyMessage));
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.HordeLockable, new HordeLockableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PickupItemSwitcher, new PickupItemSwitcherMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.Cannon, new CannonMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.PilotRotation, new PilotRotationMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.TriggerColourCycle, new TriggerColourCycleMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.MultiTriggerDisable, new TriggerDisableMessage());
		SerialisationRegistry<EntityType>.RegisterMessageType(EntityType.FireHazardSpawner, new FireHazardSpawnerMessage());
		Mailbox.Server.Initialise(m_LocalServer);
		Mailbox.Client.Initialise(m_LocalClient);
		ConnectionModeSwitcher.Initialise(m_LocalServer, m_LocalClient);
		ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Offline, null, OnOfflineMode);
		Mailbox.Server.RegisterForMessageType(MessageType.HighScores, NetworkUtils.RepeatHighScores);
		Mailbox.Server.RegisterForMessageType(MessageType.ResumeEntitySync, NetworkUtils.OnResumeEntitySyncMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.Input, m_ClientSync.OnEntityEventMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.LevelLoadByIndex, NetworkUtils.LevelLoadByIndex);
		Mailbox.Client.RegisterForMessageType(MessageType.LevelLoadByName, NetworkUtils.LevelLoadByName);
		Mailbox.Client.RegisterForMessageType(MessageType.GameProgressData, NetworkUtils.LoadGameProgressData);
		Mailbox.Client.RegisterForMessageType(MessageType.SetupCoopSession, NetworkUtils.SetupCoopSession);
		Mailbox.Client.RegisterForMessageType(MessageType.TriggerAudio, m_ClientSync.OnTriggerAudioMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.ResumeWorldObjectSync, m_ClientSync.OnResumeObjectMessageReceived<WorldObjectMessage>);
		Mailbox.Client.RegisterForMessageType(MessageType.ResumeChefPositionSync, m_ClientSync.OnResumeObjectMessageReceived<ChefPositionMessage>);
		Mailbox.Client.RegisterForMessageType(MessageType.ResumePhysicsObjectSync, m_ClientSync.OnResumeObjectMessageReceived<PhysicsObjectMessage>);
		Mailbox.Client.RegisterForMessageType(MessageType.SessionConfigSync, m_ClientSync.OnSessionConfigReceived);
	}

	private void OnOfflineMode(IConnectionModeSwitchStatus status)
	{
		UserSystemUtils.Initialise();
		m_ConnectionStatus.Initialise();
		m_DisconnectionHandler.Initialise();
		ClientGameSetup.Initialise();
		ServerGameSetup.Mode = GameMode.OnlineKitchen;
		m_LocalClient.OnLeftSession += m_DisconnectionHandler.HandleSessionConnectionLost;
		ClientUserSystem.usersChanged = (GenericVoid)Delegate.Combine(ClientUserSystem.usersChanged, new GenericVoid(NetworkUtils.SelectRandomAvatar));
	}

	[Conditional("DEBUG")]
	private void CheckBitCount()
	{
		int num = 0;
		foreach (GameState value in Enum.GetValues(typeof(GameState)))
		{
			if ((int)value > num)
			{
				num = (int)value;
			}
		}
	}

	private void Start()
	{
		m_InviteMonitor.Initialise();
	}

	private void OnDestroy()
	{
		ClientUserSystem.usersChanged = (GenericVoid)Delegate.Remove(ClientUserSystem.usersChanged, new GenericVoid(NetworkUtils.SelectRandomAvatar));
		m_EntitySerialiser.Clear();
		m_scanCoroutine = null;
		m_ServerSync.StopSynchronising();
		Mailbox.Server.UnregisterForMessageType(MessageType.HighScores, NetworkUtils.RepeatHighScores);
		Mailbox.Server.UnregisterForMessageType(MessageType.ResumeEntitySync, NetworkUtils.OnResumeEntitySyncMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.Input, m_ClientSync.OnEntityEventMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.LevelLoadByIndex, NetworkUtils.LevelLoadByIndex);
		Mailbox.Client.UnregisterForMessageType(MessageType.LevelLoadByName, NetworkUtils.LevelLoadByName);
		Mailbox.Client.UnregisterForMessageType(MessageType.GameProgressData, NetworkUtils.LoadGameProgressData);
		Mailbox.Client.UnregisterForMessageType(MessageType.SetupCoopSession, NetworkUtils.SetupCoopSession);
		Mailbox.Server.Clear();
		Mailbox.Client.Clear();
		Mailbox.Server.Shutdown();
		Mailbox.Client.Shutdown();
		m_ConnectionStatus.Shutdown();
		m_LocalClient.OnLeftSession -= m_DisconnectionHandler.HandleSessionConnectionLost;
		m_DisconnectionHandler.Shutdown();
		ClientGameSetup.Shutdown();
	}

	public void StartWorldMap()
	{
		if (IsServer())
		{
			base.gameObject.AddComponent<ServerMapLoader>();
		}
		ClientMapLoader clientMapLoader = base.gameObject.AddComponent<ClientMapLoader>();
		clientMapLoader.Initialise(m_LocalClient, this);
	}

	public void StopWorldMap()
	{
		TryRemoveComponent<ServerMapLoader>();
		TryRemoveComponent<ClientMapLoader>();
	}

	public void StartKitchen()
	{
		if (IsServer())
		{
			base.gameObject.AddComponent<ServerKitchenLoader>();
		}
		ClientKitchenLoader clientKitchenLoader = base.gameObject.AddComponent<ClientKitchenLoader>();
		clientKitchenLoader.Initialise(m_LocalClient, this);
	}

	public void StopKitchen()
	{
		TryRemoveComponent<ServerKitchenLoader>();
		TryRemoveComponent<ClientKitchenLoader>();
	}

	public void ScanEntities(Action onComplete = null)
	{
		m_scanCoroutine = AsyncScanEntities(onComplete);
	}

	private IEnumerator AsyncScanEntities(Action onComplete)
	{
		GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootObjects.Length; i++)
		{
			Component[] componentsInChildren = rootObjects[i].GetComponentsInChildren(typeof(PhysicalAttachment), true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				PhysicalAttachment physicalAttachment = (PhysicalAttachment)componentsInChildren[j];
				physicalAttachment.InactiveSetup();
			}
		}
		m_EntitySerialiser.Clear();
		m_ServerSync.StopSynchronising();
		if (IsServer())
		{
			m_EntitySerialiser.AddSynchronisedType(typeof(PlayerAnimationDecisions), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerChefSynchroniser), typeof(ClientOnTheServerChefSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(EmptyLerp), typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(MapAvatarControls), new SynchroniserConfig[3]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMapAvatarControls), typeof(ClientMapAvatarControls)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(PhysicsObjectSynchroniser), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPhysicsObjectSynchroniser), typeof(ServerClientPhysicsObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(RigidbodyMotion), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(PhysicalAttachment), new SynchroniserConfig[4]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPhysicalAttachment), typeof(ClientPhysicalAttachment)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(RendererSceneInfo)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(Projectile), new SynchroniserConfig[3]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerProjectile), typeof(ClientProjectile)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(EmptyLerp))
			});
		}
		else
		{
			m_EntitySerialiser.AddSynchronisedType(typeof(PlayerAnimationDecisions), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerChefSynchroniser), typeof(ClientChefSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(EmptyLerp), typeof(EmptyLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(MapAvatarControls), new SynchroniserConfig[3]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMapAvatarControls), typeof(ClientMapAvatarControls)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(BasicLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(PhysicsObjectSynchroniser), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPhysicsObjectSynchroniser), typeof(ClientPhysicsObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(PhysicsObjectLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(RigidbodyMotion), new SynchroniserConfig[2]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(BasicLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(PhysicalAttachment), new SynchroniserConfig[4]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPhysicalAttachment), typeof(ClientPhysicalAttachment)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(RendererSceneInfo)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(SnapLerp))
			});
			m_EntitySerialiser.AddSynchronisedType(typeof(Projectile), new SynchroniserConfig[3]
			{
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldObjectSynchroniser), typeof(ClientWorldObjectSynchroniser)),
				new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerProjectile), typeof(ClientProjectile)),
				new SynchroniserConfig(InstancesPerGameObject.Single, null, typeof(BasicLerp))
			});
		}
		m_EntitySerialiser.AddSynchronisedType(typeof(DummySynchroniserComponent), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDummySynchroniserComponent), typeof(ClientDummySynchroniserComponent)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DynamicLandscapeParenting), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDynamicLandscapeParenting), typeof(ClientDynamicLandscapeParenting)));
		m_EntitySerialiser.AddSynchronisedType(typeof(StoryLevelFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerStoryLevelFlowController), typeof(ClientStoryLevelFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TutorialIconController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTutorialIconController), typeof(ClientTutorialIconController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IconTutorialBase), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIconTutorialBase), typeof(ClientIconTutorialBase)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BossFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBossFlowController), typeof(ClientBossFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DynamicFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDynamicFlowController), typeof(ClientDynamicFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CampaignFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCampaignFlowController), typeof(ClientCampaignFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CompetitiveFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCompetitiveFlowController), typeof(ClientCompetitiveFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WorldMapFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldMapFlowController), typeof(ClientWorldMapFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MapAvatarDynamicLandscapeParenting), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMapAvatarDynamicLandscapeParenting), typeof(ClientMapAvatarDynamicLandscapeParenting)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WaterGunSpray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWaterGunSpray), typeof(ClientWaterGunSpray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Washable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWashable), typeof(ClientWashable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BellowsSpray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBellowsSpray), typeof(ClientBellowsSpray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientSpray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientSpray), typeof(ClientIngredientSpray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Backpack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBackpack), typeof(ClientBackpack)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Backpack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBackpackDispenser), typeof(ClientBackpackDispenser)));
		m_EntitySerialiser.AddSynchronisedType(typeof(LimitedFromOrderComplexity), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerLimitedFromOrderComplexity), typeof(ClientLimitedFromOrderComplexity)));
		m_EntitySerialiser.AddSynchronisedType(typeof(LimitedQuantityItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerLimitedQuantityItem), typeof(ClientLimitedQuantityItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(LimitedQuantityItemManager), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerLimitedQuantityItemManager), typeof(ClientLimitedQuantityItemManager)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachmentWindReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentWindReceiver), typeof(ClientAttachmentWindReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FireExtinguishSpray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFireExtinguishSpray), typeof(ClientFireExtinguishSpray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FlamethrowerSpray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFlamethrowerSpray), typeof(ClientFlamethrowerSpray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Flammable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFlammable), typeof(ClientFlammable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlayerControls), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerInputReceiver), typeof(ClientInputTransmitter)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlayerAttachmentCarrier), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlayerAttachmentCarrier), typeof(ClientPlayerAttachmentCarrier)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachStation), new SynchroniserConfig[2]
		{
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachStation), typeof(ClientAttachStation)),
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentCatchingProxy), typeof(ClientAttachmentCatchingProxy))
		});
		m_EntitySerialiser.AddSynchronisedType(typeof(ObjectContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerSynchroniserBase), typeof(ClientSynchroniserBase)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAttachedSpawn), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerAttachedSpawn), typeof(ClientTriggerAttachedSpawn)));
		m_EntitySerialiser.AddSynchronisedType(typeof(RubbishBin), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerRubbishBin), typeof(ClientRubbishBin)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Workstation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorkstation), typeof(ClientWorkstation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WorkableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorkableItem), typeof(ClientWorkableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlateReturnStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlateReturnStation), typeof(ClientPlateReturnStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlateStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlateStation), typeof(ClientPlateStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CleanPlateStack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCleanPlateStack), typeof(ClientCleanPlateStack)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DirtyPlateStack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDirtyPlateStack), typeof(ClientDirtyPlateStack)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Tray), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTray), typeof(ClientTray)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Plate), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlate), typeof(ClientPlate)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WashingStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWashingStation), typeof(ClientWashingStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HandlePlacementReferral), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHandlePlacementReferral), typeof(ClientHandlePlacementReferral)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HandlePickupReferral), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHandlePickupReferral), typeof(ClientHandlePickupReferral)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TrayIngredientContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTrayIngredientContainer), typeof(ClientTrayIngredientContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientContainer), typeof(ClientIngredientContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookableContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookableContainer), typeof(ClientCookableContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookablePreparationContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookablePreparationContainer), typeof(ClientCookablePreparationContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PreparationContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPreparationContainer), typeof(ClientPreparationContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MixableContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMixableContainer), typeof(ClientMixableContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ItemContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerItemContainer), typeof(ClientItemContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(LadleContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerLadleContainer), typeof(ClientLadleContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WokEffectsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWokEffectsCosmeticDecisions), typeof(ClientWokEffectsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ContentsDisposalBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerContentsDisposalBehaviour), typeof(ClientContentsDisposalBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientDisposalBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientDisposalBehaviour), typeof(ClientIngredientDisposalBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TrayPlacementContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTrayPlacementContainer), typeof(ClientTrayPlacementContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlacementContainer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlacementContainer), typeof(ClientPlacementContainer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientContentGUI), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientContentGUI), typeof(ClientIngredientContentGUI)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ProjectileSpawner), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerProjectileSpawner), typeof(ClientProjectileSpawner)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FireHazardSpawner), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFireHazardSpawner), typeof(ClientFireHazardSpawner)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FireHazard), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFireHazard), typeof(ClientFireHazard)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AnticipateInteractionHighlight), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAnticipateInteractionHighlight), typeof(ClientAnticipateInteractionHighlight)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookingEffectsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookingEffectsCosmeticDecisions), typeof(ClientCookingEffectsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookingHandler), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookingHandler), typeof(ClientCookingHandler)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ContentsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerContentsCosmeticDecisions), typeof(ClientContentsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CakeTinContentsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCakeTinContentsCosmeticDecisions), typeof(ClientCakeTinContentsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FryingContentsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFryingContentsCosmeticDecisions), typeof(ClientFryingContentsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FlambeCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFlambeCosmeticDecisions), typeof(ClientFlambeCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookableIngredient), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookableIngredient), typeof(ClientCookableIngredient)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HeatedStation), new SynchroniserConfig[2]
		{
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeatedStation), typeof(ClientHeatedStation)),
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentCatchingProxy), typeof(ClientAttachmentCatchingProxy))
		});
		m_EntitySerialiser.AddSynchronisedType(typeof(HeatedCookingStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeatedCookingStation), typeof(ClientHeatedCookingStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HeatedStationGUI), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeatedStationGUI), typeof(ClientHeatedStationGUI)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BarbequeCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBarbequeCosmeticDecisions), typeof(ClientBarbequeCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BellowsCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBellowsCosmeticDecisions), typeof(ClientBellowsCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WaterGunCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWaterGunCosmeticDecisions), typeof(ClientWaterGunCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BlenderCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBlenderCosmeticDecisions), typeof(ClientBlenderCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CampfireCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCampfireCosmeticDecisions), typeof(ClientCampfireCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ToastingForkCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerToastingForkCosmeticDecisions), typeof(ClientToastingForkCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookingStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookingStation), typeof(ClientCookingStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MixingStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMixingStation), typeof(ClientMixingStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MixingHandler), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMixingHandler), typeof(ClientMixingHandler)));
		m_EntitySerialiser.AddSynchronisedType(typeof(OvenCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerOvenCosmeticDecisions), typeof(ClientOvenCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ChoppingStationCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerChoppingStationCosmeticDecisions), typeof(ClientChoppingStationCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ConveyorStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerConveyorStation), typeof(ClientConveyorStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TabletopConveyenceReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTabletopConveyenceReceiver), typeof(ClientTabletopConveyenceReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalConveyenceReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalConveyenceReceiver), typeof(ClientTeleportalConveyenceReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ConveyorBeltCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerConveyorBeltCosmeticDecisions), typeof(ClientConveyorBeltCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAnimationOnConveyor), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTriggerAnimationOnConveyor), typeof(ClientTriggerAnimationOnConveyor)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachedOrderCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachedOrderCosmeticDecisions), typeof(ClientAttachedOrderCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PickupItemSpawner), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPickupItemSpawner), typeof(ClientPickupItemSpawner)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlacementItemSpawner), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlacementItemSpawner), typeof(ClientPlacementItemSpawner)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ItemCrateCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerItemCrateCosmeticDecisions), typeof(ClientItemCrateCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Teleportal), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportal), typeof(ClientTeleportal)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalPlayerSender), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalPlayerSender), typeof(ClientTeleportalPlayerSender)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalPlayerReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalPlayerReceiver), typeof(ClientTeleportalPlayerReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalAttachmentSender), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalAttachmentSender), typeof(ClientTeleportalAttachmentSender)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalAttachmentReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalAttachmentReceiver), typeof(ClientTeleportalAttachmentReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalCosmeticDecisions), typeof(ClientTeleportalCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportablePlayer), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportablePlayer), typeof(ClientTeleportablePlayer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportableItem), typeof(ClientTeleportableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Terminal), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTerminal), typeof(ClientTerminal)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TerminalCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTerminalCosmeticDecisions), typeof(ClientTerminalCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PilotRotation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPilotRotation), typeof(ClientPilotRotation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PilotMovement), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPilotMovement), typeof(ClientPilotMovement)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MovingPlatformCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMovingPlatformCosmeticDecisions), typeof(ClientMovingPlatformCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ExtinguishCollider), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerExtinguishCollider), typeof(ClientExtinguishCollider)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PushableObject), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPushableObject), typeof(ClientPushableObject)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookingRegion), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookingRegion), typeof(ClientCookingRegion)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerZone), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerZone), typeof(ClientTriggerZone)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerTimer), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerTimer), typeof(ClientTriggerTimer)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAdapter), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerAdapter), typeof(ClientTriggerAdapter)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerCounter), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerCounter), typeof(ClientTriggerCounter)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MultiTriggerAdapter), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerMultiTriggerAdapter), typeof(ClientMultiTriggerAdapter)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerOnObject), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerOnObject), typeof(ClientTriggerOnObject)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerDisableScript), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerDisableScript), typeof(ClientTriggerDisableScript)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerDestroy), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerDestroy), typeof(ClientTriggerDestroy)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerQueue), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerQueue), typeof(ClientTriggerQueue)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAnimationQueue), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerAnimationQueue), typeof(ClientTriggerAnimationQueue)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAnimationCoordinator), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTriggerAnimationCoordinator), typeof(ClientTriggerAnimationCoordinator)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerConveyorAdjacentUpdate), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerConveyorAdjacentUpdate), typeof(ClientTriggerConveyorAdjacentUpdate)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerAnimatorSetVariable), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerAnimatorSetVariable), typeof(ClientTriggerAnimatorSetVariable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerOnAnimator), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerOnAnimator), typeof(ClientTriggerOnAnimator)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerToggleOnAnimator), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerToggleOnAnimator), typeof(ClientTriggerToggleOnAnimator)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerMoveSpawnPoints), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerMoveSpawnPoints), typeof(ClientTriggerMoveSpawnPoints)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerKillAttachments), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerKillAttachments), typeof(ClientTriggerKillAttachments)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerDialogue), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerDialogue), typeof(ClientTriggerDialogue)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CollisionTrigger), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerCollisionTrigger), typeof(ClientCollisionTrigger)));
		m_EntitySerialiser.AddSynchronisedType(typeof(SwitchCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerSwitchCosmeticDecisions), typeof(ClientSwitchCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PressureSwitchCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPressureSwitchCosmeticDecisions), typeof(ClientPressureSwitchCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ToggleSwitchCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerToggleSwitchCosmeticDecisions), typeof(ClientToggleSwitchCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(UsableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerUsableItem), typeof(ClientUsableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Interactable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerInteractable), typeof(ClientInteractable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(SwitchStation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerSwitchStation), typeof(ClientSwitchStation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientMeshVisibility), typeof(ClientIngredientMeshVisibility)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HeldItemsMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeldItemsMeshVisibility), typeof(ClientHeldItemsMeshVisibility)));
		//m_EntitySerialiser.AddSynchronisedType(typeof(HeldItemMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeldItemMeshVisibility), typeof(ClientHeldItemsMeshVisibility)));
		// patch
		m_EntitySerialiser.AddSynchronisedType(typeof(HeldItemMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHeldItemMeshVisibility), typeof(ClientHeldItemMeshVisibility)));
        // patch
		m_EntitySerialiser.AddSynchronisedType(typeof(HatMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHatMeshVisibility), typeof(ClientHatMeshVisibility)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TailMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTailMeshVisibility), typeof(ClientTailMeshVisibility)));
		m_EntitySerialiser.AddSynchronisedType(typeof(RespawnCollider), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerRespawnCollider), typeof(ClientRespawnCollider)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlayerRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlayerRespawnBehaviour), typeof(ClientPlayerRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DirtyPlateStackUtensilRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDirtyPlateStackUtensilRespawnBehaviour), typeof(ClientUtensilRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CleanPlateUtensilRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCleanPlateUtensilRespawnBehaviour), typeof(ClientUtensilRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CookingUtensilRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCookingUtensilRespawnBehaviour), typeof(ClientUtensilRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(UtensilRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerUtensilRespawnBehaviour), typeof(ClientUtensilRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AutoDestructEmptyPlateStack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAutoDestructEmptyPlateStack), typeof(ClientAutoDestructEmptyPlateStack)));
		m_EntitySerialiser.AddSynchronisedType(typeof(EmoteWheel), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerEmoteWheel), typeof(ClientEmoteWheel)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlayerControlsImpl_Default), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlayerControlsImpl_Default), typeof(ClientPlayerControlsImpl_Default)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PreventPlacement), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPreventPlacement), typeof(ClientPreventPlacement)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Stack), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerStack), typeof(ClientStack)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CarryableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCarryableItem), typeof(ClientCarryableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(ThrowableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerThrowableItem), typeof(ClientThrowableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CatchableItem), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCatchableItem), typeof(ClientCatchableItem)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachmentThrower), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentThrower), typeof(ClientAttachmentThrower)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachmentCatcher), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentCatcher), typeof(ClientAttachmentCatcher)));
		m_EntitySerialiser.AddSynchronisedType(typeof(IngredientCatcher), new SynchroniserConfig[2]
		{
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientCatcher), typeof(ClientIngredientCatcher)),
			new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentCatchingProxy), typeof(ClientAttachmentCatchingProxy))
		});
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachmentCatchingProxy), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachmentCatchingProxy), typeof(ClientAttachmentCatchingProxy)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CutsceneController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCutsceneController), typeof(ClientCutsceneController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DialogueController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDialogueController), typeof(ClientDialogueController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TutorialPopupController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTutorialPopupController), typeof(ClientTutorialPopupController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(StoryOnionKingCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerStoryOnionKingCosmeticDecisions), typeof(ClientStoryOnionKingCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(StoryKevinCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerStoryKevinCosmeticDecisions), typeof(ClientStoryKevinCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MultiLevelMiniPortalMapNode), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMultiLevelMiniPortalMapNode), typeof(ClientMultiLevelMiniPortalMapNode)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MiniLevelPortalMapNode), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerMiniLevelPortalMapNode), typeof(ClientMiniLevelPortalMapNode)));
		m_EntitySerialiser.AddSynchronisedType(typeof(LevelPortalMapNode), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerLevelPortalMapNode), typeof(ClientLevelPortalMapNode)));
		m_EntitySerialiser.AddSynchronisedType(typeof(SwitchMapNode), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerSwitchMapNode), typeof(ClientSwitchMapNode)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WorldMapSwitch), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldMapSwitch), typeof(ClientWorldMapSwitch)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WorldMapTeleportal), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldMapTeleportal), typeof(ClientWorldMapTeleportal)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportableMapAvatar), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportableMapAvatar), typeof(ClientTeleportableMapAvatar)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalMapAvatarSender), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalMapAvatarSender), typeof(ClientTeleportalMapAvatarSender)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TeleportalMapAvatarReceiver), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTeleportalMapAvatarReceiver), typeof(ClientTeleportalMapAvatarReceiver)));
		m_EntitySerialiser.AddSynchronisedType(typeof(WorldMapInfoPopup), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerWorldMapInfoPopup), typeof(ClientWorldMapInfoPopup)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BackpackRespawnBehaviour), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBackpackRespawnBehaviour), typeof(ClientBackpackRespawnBehaviour)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BackpackCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBackpackCosmeticDecisions), typeof(ClientBackpackCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AutoWorkstation), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAutoWorkstation), typeof(ClientAutoWorkstation)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FurnaceCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFurnaceCosmeticDecisions), typeof(ClientFurnaceCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(FurnaceOvenCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerFurnaceOvenCosmeticDecisions), typeof(ClientFurnaceOvenCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CoalScuttleCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCoalScuttleCosmeticDecisions), typeof(ClientCoalScuttleCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeEnemy), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeEnemy), typeof(ClientHordeEnemy)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeEnemyCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeEnemyCosmeticDecisions), typeof(ClientHordeEnemyCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeTarget), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeTarget), typeof(ClientHordeTarget)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeTargetCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeTargetCosmeticDecisions), typeof(ClientHordeTargetCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeLockable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeLockable), typeof(ClientHordeLockable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeLockableCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeLockableCosmeticDecisions), typeof(ClientHordeLockableCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(HordeFlowController), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerHordeFlowController), typeof(ClientHordeFlowController)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerOnAttach), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTriggerOnAttach), typeof(ClientTriggerOnAttach)));
		m_EntitySerialiser.AddSynchronisedType(typeof(AttachItemSpawner), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerAttachItemSpawner), typeof(ClientAttachItemSpawner)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PickupItemSwitcher), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPickupItemSwitcher), typeof(ClientPickupItemSwitcher)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlacementItemSwitcher), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlacementItemSwitcher), typeof(ClientPlacementItemSwitcher)));
		m_EntitySerialiser.AddSynchronisedType(typeof(DrinksMachineCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerDrinksMachineCosmeticDecisions), typeof(ClientDrinksMachineCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CondimentDispenserCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCondimentDispenserCosmeticDecisions), typeof(ClientCondimentDispenserCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CannonSessionInteractable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCannonSessionInteractable), typeof(ClientCannonSessionInteractable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(Cannon), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCannon), typeof(ClientCannon)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CannonPlayerHandler), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCannonPlayerHandler), typeof(ClientCannonPlayerHandler)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TrayCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTrayCosmeticDecisions), typeof(ClientTrayCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(PlacementInteractable), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerPlacementInteractable), typeof(ClientPlacementInteractable)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BodyMeshVisibility), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerBodyMeshVisibility), typeof(ClientBodyMeshVisibility)));
		m_EntitySerialiser.AddSynchronisedType(typeof(CannonCosmeticDecisions), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerCannonCosmeticDecisions), typeof(ClientCannonCosmeticDecisions)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerColourCycle), new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerTriggerColourCycle), typeof(ClientTriggerColourCycle)));
		m_EntitySerialiser.AddSynchronisedType(typeof(MultiTriggerDisableScript), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerMultiTriggerDisableScript), typeof(ClientMultiTriggerDisableScript)));
		m_EntitySerialiser.AddSynchronisedType(typeof(TriggerWithCooldown), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerTriggerWithCooldown), typeof(ClientTriggerWithCooldown)));
		m_EntitySerialiser.AddSynchronisedType(typeof(BoundContainer), new SynchroniserConfig(InstancesPerGameObject.Multiple, typeof(ServerBoundContainerBehaviour), typeof(ClientBoundContainerBehaviour)));
		if (IsServer())
		{
			base.gameObject.AddComponent<ServerPlayerRespawnManager>();
			m_ServerSync.Initialise();
		}
		IEnumerator setupRoutine = m_EntitySerialiser.SetupSynchronisation(this);
		while (setupRoutine.MoveNext())
		{
			yield return null;
		}
		if (IsServer())
		{
			ServerChefSynchroniser[] array = GameObjectUtils.FindComponentsOfTypeInScene<ServerChefSynchroniser>();
			for (int k = 0; k < array.Length; k++)
			{
				EntitySerialisationEntry entry = EntitySerialisationRegistry.GetEntry(array[k].gameObject);
				m_ServerSync.AddToFastList(entry);
			}
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	public void StartSynchronisation()
	{
		Mailbox.Client.RegisterForMessageType(MessageType.EntitySynchronisation, m_ClientSync.OnEntitySynchronisationMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.EntityEvent, m_ClientSync.OnEntityEventMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.SpawnEntity, m_ClientSync.OnSpawnEntityMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.DestroyEntity, m_ClientSync.OnDestroyEntityMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.SpawnPhysicalAttachment, m_ClientSync.OnSpawnPhysicalAttachmentMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.DestroyChef, m_ClientSync.OnDestroyChefMessageReceived);
		Mailbox.Client.RegisterForMessageType(MessageType.DestroyEntities, m_ClientSync.OnDestroyEntitiesMessageReceived);
		m_EntitySerialiser.StartSynchronisation();
		if (m_ServerSync != null && IsServer())
		{
			m_ServerSync.StartSynchronising();
		}
		m_bSyncActive = true;

        // patch
		PseudoPrefabManager.SetupAfterStartSynchronisingAllPseudoPrefabs();
        // patch
    }

    public static bool IsSynchronisationActive()
	{
		return m_bSyncActive;
	}

	public void StopSynchronisation()
	{
		m_bSyncActive = false;
		m_EntitySerialiser.StopSynchronisation();
		m_ServerSync.StopSynchronising();
		m_EntitySerialiser.Clear();
		m_scanCoroutine = null;
		TryRemoveComponent<ServerPlayerRespawnManager>();
		m_LocalServer.GetUserSystem().InvalidateEntities();
		m_ClientSync.CleanUp();
		Mailbox.Client.UnregisterForMessageType(MessageType.EntityEvent, m_ClientSync.OnEntityEventMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.EntitySynchronisation, m_ClientSync.OnEntitySynchronisationMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.SpawnEntity, m_ClientSync.OnSpawnEntityMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.DestroyEntity, m_ClientSync.OnDestroyEntityMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.SpawnPhysicalAttachment, m_ClientSync.OnSpawnPhysicalAttachmentMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.DestroyChef, m_ClientSync.OnDestroyChefMessageReceived);
		Mailbox.Client.UnregisterForMessageType(MessageType.DestroyEntities, m_ClientSync.OnDestroyEntitiesMessageReceived);
	}

	public ConnectionStats GetClientConnectionStats(bool bReliable)
	{
		return m_LocalClient.GetConnectionStats(bReliable);
	}

	public FastList<ConnectionStats> GetServerConnectionStats(bool bReliable)
	{
		return m_LocalServer.GetAllConnectionStats(bReliable);
	}

	private void TryRemoveComponent<T>() where T : Component
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			Component component = base.gameObject.GetComponent<T>();
			if (null != component)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}

	public void Update()
	{
		m_InviteMonitor.Update();
		ConnectionModeSwitcher.Update();
		ClientUserSystem.Update();
		if (IsServer())
		{
			ServerTime.Update();
			m_LocalServer.Update();
		}
		m_LocalClient.Update();
		if (m_bSyncActive)
		{
			FastList<EntitySerialisationEntry> entitiesList = EntitySerialisationRegistry.m_EntitiesList;
			int count = entitiesList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!m_bSyncActive)
				{
					break;
				}
				EntitySerialisationEntry entitySerialisationEntry = entitiesList._items[i];
				FastList<ServerSynchroniser> serverSynchronisedComponents = entitySerialisationEntry.m_ServerSynchronisedComponents;
				int count2 = serverSynchronisedComponents.Count;
				for (int j = 0; j < count2; j++)
				{
					if (!m_bSyncActive)
					{
						break;
					}
					ServerSynchroniser serverSynchroniser = serverSynchronisedComponents._items[j];
					if (serverSynchroniser.IsSynchronising())
					{
						serverSynchronisedComponents._items[j].UpdateSynchronising();
					}
				}
				if (!m_bSyncActive)
				{
					continue;
				}
				FastList<ClientSynchroniser> clientSynchronisedComponents = entitySerialisationEntry.m_ClientSynchronisedComponents;
				int count3 = clientSynchronisedComponents.Count;
				for (int k = 0; k < count3; k++)
				{
					if (!m_bSyncActive)
					{
						break;
					}
					ClientSynchroniser clientSynchroniser = clientSynchronisedComponents._items[k];
					if (clientSynchroniser.IsSynchronising())
					{
						clientSynchronisedComponents._items[k].UpdateSynchronising();
					}
				}
			}
			if (IsServer() && m_bSyncActive)
			{
				m_ServerSync.Update();
			}
			m_ClientSync.Update();
		}
		ServerMessenger.DeferredLevelLoad();
		if (m_scanCoroutine != null && !m_scanCoroutine.MoveNext())
		{
			m_scanCoroutine = null;
		}
	}

	public void LateUpdate()
	{
		if (IsServer())
		{
			m_LocalServer.Dispatch();
		}
		m_LocalClient.Dispatch();
	}

	public void SetTracker(NetworkMessageTracker tracker)
	{
		m_LocalServer.SetTracker(tracker);
		m_LocalClient.SetTracker(tracker);
		m_ServerSync.SetTracker(tracker);
		m_ClientSync.SetTracker(tracker);
	}

	public void SwitchNodeType(NodeType type)
	{
		if (type != m_NodeType)
		{
			m_NodeType = type;
			if (m_NodeType == NodeType.Server)
			{
				m_LocalServer.GetUserSystem().Initialise();
			}
			else
			{
				m_LocalServer.GetUserSystem().Shutdown();
			}
		}
	}

	private bool IsServer()
	{
		return m_NodeType == NodeType.Server;
	}

	public void SetLatencyMeasurePaused(bool bPaused)
	{
		if (m_LocalServer != null)
		{
			m_LocalServer.SetLatencyTestPaused(bPaused);
		}
		if (m_LocalClient != null)
		{
			m_LocalClient.SetLatencyTestPaused(bPaused);
		}
	}
}
