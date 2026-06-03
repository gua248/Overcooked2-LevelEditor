using AssetBundles;
using System;
using System.Collections.Generic;
using System.Reflection;
using T17.Analytics;
using Team17.Online;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameUtils
{
	private static GameDebugConfig s_config;

	private static string s_sceneNameToLoad;

	private static Helper s_AnalyticsHelper;

	private static bool s_AnalyticsEnabled = true;

	private static int s_LastMatchedOrderIndex = -1;

	public static bool s_RoomSearch_NoneAvailable;

	public static Helper GetT17AnalyticsHelper()
	{
		return s_AnalyticsHelper;
	}

	public static void EnableAnalytics(bool enable)
	{
		s_AnalyticsEnabled = enable;
		if (s_AnalyticsHelper != null)
		{
			s_AnalyticsHelper.EnableAnalytics(enable);
		}
	}

	public static GameObject GetGameEnvironment()
	{
		return GameObject.FindGameObjectWithTag("GameController");
	}

	public static GameObject GetGameMetaEnvironment()
	{
		return GameObject.FindGameObjectWithTag("GameMetaEnvironment");
	}

	public static GameSession GetGameSession()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("GameSession");
		if (gameObject != null)
		{
			return gameObject.RequireComponent<GameSession>();
		}
		return null;
	}

	public static int GetLevelID()
	{
		GameSession gameSession = GetGameSession();
		SceneDirectoryData sceneDirectory = gameSession.Progress.GetSceneDirectory();
		GameSession.GameLevelSettings levelSettings = gameSession.LevelSettings;
		Predicate<SceneDirectoryData.SceneDirectoryEntry> matchFunction = (SceneDirectoryData.SceneDirectoryEntry x) => x.SceneVarients.Contains(levelSettings.SceneDirectoryVarientEntry);
		return sceneDirectory.Scenes.FindIndex_Predicate(matchFunction);
	}

	public static MetaGameProgress GetMetaGameProgress()
	{
		SaveManager saveManager = RequireManager<SaveManager>();
		return saveManager.GetMetaGameProgress();
	}

	public static AvatarDirectoryData GetAvatarDirectoryData()
	{
		MetaGameProgress metaGameProgress = GetMetaGameProgress();
		if (metaGameProgress != null)
		{
			return metaGameProgress.AvatarDirectory;
		}
		return null;
	}

	public static string DebugGetState()
	{
		GameSession gameSession = GetGameSession();
		if (gameSession != null && gameSession.LevelSettings != null && gameSession.LevelSettings.SceneDirectoryVarientEntry != null)
		{
			return "InLevel - " + gameSession.LevelSettings.SceneDirectoryVarientEntry.SceneName;
		}
		return "OutsideLevel";
	}

	public static GameDebugManager GetDebugManager()
	{
		return RequireManager<GameDebugManager>();
	}

	public static GridManager GetGridManager(Transform _areaReference)
	{
		if (_areaReference != null)
		{
			GridManager gridManager = _areaReference.gameObject.RequestComponentUpwardsRecursive<GridManager>();
			if (gridManager != null)
			{
				return gridManager;
			}
		}
		return RequireManagerInterface<GridManager>();
	}

	public static GridNavSpace GetGridNavSpace()
	{
		return RequireManager<GridNavSpace>();
	}

	public static IFlowController GetFlowController()
	{
		return RequireManagerInterface<IFlowController>();
	}

	public static GameDebugConfig GetDebugConfig()
	{
		if ((bool)s_config)
		{
			return s_config;
		}
		GameDebugManager debugManager = GetDebugManager();
		s_config = debugManager.GetDebugConfig();
		return s_config;
	}

	public static T RequireManager<T>() where T : Manager
	{
		return RequestManager<T>();
	}

	public static T RequestManager<T>() where T : Manager
	{
		GameObject[] array = new GameObject[2]
		{
			GetGameEnvironment(),
			GetGameMetaEnvironment()
		};
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			ManagerDirectory managerDirectory = gameObject.RequestComponent<ManagerDirectory>();
			if (managerDirectory != null)
			{
				T val = managerDirectory.RequestManager<T>();
				if (val != null)
				{
					return val;
				}
			}
		}
		return (T)null;
	}

	public static T RequestManagerInterface<T>() where T : class
	{
		T val = RequestManagerInterfaceFromEnvironment<T>(GetGameEnvironment());
		if (val == null)
		{
			val = RequestManagerInterfaceFromEnvironment<T>(GetGameMetaEnvironment());
		}
		return val;
	}

	public static T RequestManagerInterfaceFromEnvironment<T>(GameObject environment) where T : class
	{
		if (environment != null)
		{
			ManagerDirectory managerDirectory = environment.RequestComponent<ManagerDirectory>();
			if (managerDirectory != null)
			{
				T val = managerDirectory.RequestManagerInterface<T>();
				if (val != null)
				{
					return val;
				}
			}
		}
		return (T)null;
	}

	public static T RequireManagerInterface<T>() where T : class
	{
		return RequestManagerInterface<T>();
	}

	public static void EnsureBootstrapSetup()
	{
		BootstrapManager bootstrapManager = UnityEngine.Object.FindObjectOfType<BootstrapManager>();
		if ((bool)bootstrapManager)
		{
			bootstrapManager.EnsureSetup();
		}
	}

	public static LevelConfigBase GetLevelConfig()
	{
		LevelConfigBase levelConfigBase = null;
		IFlowController flowController = RequestManagerInterface<IFlowController>();
		if (flowController != null)
		{
			levelConfigBase = flowController.GetLevelConfig();
		}
		if (levelConfigBase == null)
		{
			GameSession.GameLevelSettings levelSettings = GetGameSession().LevelSettings;
			if (levelSettings != null && levelSettings.SceneDirectoryVarientEntry != null)
			{
				levelConfigBase = levelSettings.SceneDirectoryVarientEntry.LevelConfig;
			}
		}
		return levelConfigBase;
	}

	public static GameConfig GetGameConfig()
	{
		IFlowController flowController = RequireManagerInterface<IFlowController>();
		return flowController.GetGameConfig();
	}

	public static AudioSource TriggerAudio(GameOneShotAudioTag _audio, int _layer)
	{
		AudioManager audioManager = RequireManager<AudioManager>();
		return audioManager.TriggerAudio(_audio, _layer);
	}

	public static AudioSource StartAudio(GameLoopingAudioTag _audio, object _token, int _layer)
	{
		AudioManager audioManager = RequireManager<AudioManager>();
		return audioManager.StartAudio(_audio, _token, _layer);
	}

	public static void StopAudio(GameLoopingAudioTag _audio, object _token)
	{
		AudioManager audioManager = RequireManager<AudioManager>();
		if ((bool)audioManager)
		{
			audioManager.StopAudio(_audio, _token);
		}
	}

	public static void TriggerNXRumble(PlayerInputLookup.Player _player, GameOneShotAudioTag _audio)
	{
	}

	public static void StartNXRumble(PlayerInputLookup.Player _player, GameLoopingAudioTag _audio)
	{
	}

	public static void StopNXRumble(PlayerInputLookup.Player _player, GameLoopingAudioTag _audio)
	{
	}

	public static void TriggerNXRumble(GameOneShotAudioTag _audio)
	{
	}

	public static void StartNXRumble(GameLoopingAudioTag _audio)
	{
	}

	public static void StopNXRumble(GameLoopingAudioTag _audio)
	{
	}

	public static void StopAllNXRumblers()
	{
	}

	public static GameObject GetNamedCanvas(string _name)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Canvas");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name == _name)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject InstantiateUIController(GameObject _source, string _canvasName)
	{
		GameObject namedCanvas = GetNamedCanvas(_canvasName);
		return InstantiateUIController(_source, namedCanvas.transform as RectTransform);
	}

	public static GameObject InstantiateUIControllerOnScalingHUDCanvas(GameObject _source)
	{
		GameObject namedCanvas = GetNamedCanvas("ScalingHUDCanvas");
		Transform transform = namedCanvas.transform.Find("SafeZoneElements");
		return InstantiateUIController(_source, transform as RectTransform);
	}

	public static GameObject InstantiateUIController(GameObject _source, RectTransform _parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_source);
		gameObject.transform.SetParent(_parent, false);
		RectTransform rectTransform = gameObject.RequireComponent<RectTransform>();
		rectTransform.localScale = _source.transform.localScale;
		return gameObject;
	}

	public static GameObject InstantiateHoverIconUIController<T>(out T controller, GameObject _source, Transform _follower, string _canvasName, Vector3 _offset = default(Vector3)) where T : HoverIconUIController
	{
		GameObject gameObject = InstantiateUIController(_source, _canvasName);
		controller = gameObject.RequireComponent<T>();
		controller.SetFollowTransform(_follower, _offset);
		return gameObject;
	}

	public static GameObject InstantiateHoverIconUIController<T>(out T controller, GameObject _source, Transform _follower, RectTransform _parent, Vector3 _offset = default(Vector3)) where T : HoverIconUIController
	{
		GameObject gameObject = InstantiateUIController(_source, _parent);
		controller = gameObject.RequireComponent<T>();
		controller.SetFollowTransform(_follower, _offset);
		return gameObject;
	}

	public static GameObject InstantiateHoverIconUIController(GameObject _source, Transform _follower, string _canvasName, Vector3 _offset = default(Vector3))
	{
		GameObject gameObject = InstantiateUIController(_source, _canvasName);
		HoverIconUIController hoverIconUIController = gameObject.RequireComponent<HoverIconUIController>();
		hoverIconUIController.SetFollowTransform(_follower, _offset);
		return gameObject;
	}

	public static ParticleSystem InstantiatePFX(this ParticleSystem _pfxPrefab, Transform _parent)
	{
        GameObject obj = _pfxPrefab.gameObject.InstantiateOnParent(_parent);
		ParticleSystem particleSystem = obj.RequireComponent<ParticleSystem>();
		if (particleSystem.main.playOnAwake)
		{
			particleSystem.RestartPFX();
			particleSystem.gameObject.RequireComponent<AutoDestructParticleSystem>();
		}
		return particleSystem;
	}

	public static ParticleSystem InstantiatePFX(this ParticleSystem _pfxPrefab, Vector3 _position)
	{
        // patch
        if (_pfxPrefab == null) return null;
        // patch

        GameObject gameObject = _pfxPrefab.gameObject.InstantiateOnParent(null);
		gameObject.transform.position = _position + _pfxPrefab.transform.position;
		ParticleSystem particleSystem = gameObject.RequireComponent<ParticleSystem>();
		if (particleSystem.main.playOnAwake)
		{
			particleSystem.RestartPFX();
			particleSystem.gameObject.RequireComponent<AutoDestructParticleSystem>();
		}
		return particleSystem;
	}

	public static void RestartPFX(this ParticleSystem _pfx)
	{
		_pfx.Clear();
		_pfx.Simulate(0f, false, true);
		_pfx.Play();
	}

	private static OrderDefinitionNode[] GetAllOrderNodes()
	{
		OrderDefinitionNode[] array = null;
		IRecipeListCache recipeListCache = RequestManagerInterface<IRecipeListCache>();
		if (recipeListCache != null)
		{
			return recipeListCache.GetCachedRecipeList();
		}
		KitchenLevelConfigBase kitchenLevelConfigBase = GetLevelConfig() as KitchenLevelConfigBase;
		if (kitchenLevelConfigBase != null && kitchenLevelConfigBase.m_recipeMatchingList != null)
		{
			array = kitchenLevelConfigBase.m_recipeMatchingList.m_recipes;
			for (int i = 0; i < kitchenLevelConfigBase.m_recipeMatchingList.m_includeLists.Length; i++)
			{
				array = array.Union(kitchenLevelConfigBase.m_recipeMatchingList.m_includeLists[i].m_recipes);
			}
		}
		return array;
	}

	private static AssembledDefinitionNode[] GetAllOrderNodesSimplified()
	{
		IRecipeListCache recipeListCache = RequestManagerInterface<IRecipeListCache>();
		if (recipeListCache != null)
		{
			return recipeListCache.GetCachedAssembledRecipes();
		}
		KitchenLevelConfigBase kitchenLevelConfigBase = GetLevelConfig() as KitchenLevelConfigBase;
		OrderDefinitionNode[] allOrderNodes = GetAllOrderNodes();
		AssembledDefinitionNode[] array = new AssembledDefinitionNode[allOrderNodes.Length];
		for (int i = 0; i < allOrderNodes.Length; i++)
		{
			array[i] = allOrderNodes[i].Convert().Simpilfy();
		}
		return array;
	}

	private static CookingStepData[] GetAllCookingSteps()
	{
		CookingStepData[] array = null;
		IRecipeListCache recipeListCache = RequestManagerInterface<IRecipeListCache>();
		if (recipeListCache != null)
		{
			return recipeListCache.GetCachedCookingSteps();
		}
		KitchenLevelConfigBase kitchenLevelConfigBase = GetLevelConfig() as KitchenLevelConfigBase;
		if (kitchenLevelConfigBase != null && kitchenLevelConfigBase.m_recipeMatchingList != null)
		{
			array = kitchenLevelConfigBase.m_recipeMatchingList.m_cookingSteps;
			for (int i = 0; i < kitchenLevelConfigBase.m_recipeMatchingList.m_includeLists.Length; i++)
			{
				array = array.Union(kitchenLevelConfigBase.m_recipeMatchingList.m_includeLists[i].m_cookingSteps);
			}
		}
		return array;
	}

	public static GameObject GetOrderPlatingPrefab(AssembledDefinitionNode _node, PlatingStepData _platingStep)
	{
		OrderDefinitionNode[] allOrderNodes = GetAllOrderNodes();
		AssembledDefinitionNode[] allOrderNodesSimplified = GetAllOrderNodesSimplified();
		if (_node == null || _platingStep == null || allOrderNodes == null || allOrderNodesSimplified == null)
		{
			return null;
		}
		AssembledDefinitionNode simpleNode = _node.Simpilfy();
		if (s_LastMatchedOrderIndex >= 0 && s_LastMatchedOrderIndex < allOrderNodes.Length && allOrderNodes[s_LastMatchedOrderIndex].m_platingStep == _platingStep && AssembledDefinitionNode.MatchingAlreadySimple(simpleNode, allOrderNodesSimplified[s_LastMatchedOrderIndex]))
		{
			return allOrderNodes[s_LastMatchedOrderIndex].m_platingPrefab;
		}
		for (int i = 0; i < allOrderNodes.Length; i++)
		{
			if (allOrderNodes[i].m_platingPrefab != null && allOrderNodes[i].m_platingStep == _platingStep && AssembledDefinitionNode.MatchingAlreadySimple(simpleNode, allOrderNodesSimplified[i]))
			{
				s_LastMatchedOrderIndex = i;
				return allOrderNodes[i].m_platingPrefab;
			}
		}
		return null;
	}

	public static GameObject GetOrderPlatingPrefab(OrderDefinitionNode _node, PlatingStepData _platingStep)
	{
		return GetOrderPlatingPrefab(_node.Simpilfy(), _platingStep);
	}

	public static bool IsValidRecipe(AssembledDefinitionNode _node)
	{
		AssembledDefinitionNode[] allOrderNodesSimplified = GetAllOrderNodesSimplified();
		if (_node == null || allOrderNodesSimplified == null)
		{
			return false;
		}
		AssembledDefinitionNode simpleNode = _node.Simpilfy();
		if (s_LastMatchedOrderIndex >= 0 && s_LastMatchedOrderIndex < allOrderNodesSimplified.Length && AssembledDefinitionNode.MatchingAlreadySimple(simpleNode, allOrderNodesSimplified[s_LastMatchedOrderIndex]))
		{
			return true;
		}
		for (int i = 0; i < allOrderNodesSimplified.Length; i++)
		{
			if (AssembledDefinitionNode.MatchingAlreadySimple(simpleNode, allOrderNodesSimplified[i]))
			{
				s_LastMatchedOrderIndex = i;
				return true;
			}
		}
		return false;
	}

	public static OrderDefinitionNode GetOrderDefinitionNode(int ID)
	{
		OrderDefinitionNode[] allOrderNodes = GetAllOrderNodes();
		for (int i = 0; i < allOrderNodes.Length; i++)
		{
			if (allOrderNodes[i].m_uID == ID)
			{
				return allOrderNodes[i];
			}
		}
		return null;
	}

	public static CookingStepData GetCookingStepData(int ID)
	{
		CookingStepData[] allCookingSteps = GetAllCookingSteps();
		for (int i = 0; i < allCookingSteps.Length; i++)
		{
			if (allCookingSteps[i].m_uID == ID)
			{
				return allCookingSteps[i];
			}
		}
		return null;
	}

	public static GameObject[] GetAllIngredients()
	{
		GameObject[] a = GameObject.FindGameObjectsWithTag("Pre-Ingredient");
		GameObject[] b = GameObject.FindGameObjectsWithTag("Ingredient");
		return a.Union(b);
	}

	public static GameObject[] GetPreIngredients(OrderDefinitionNode _orderToBe)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Pre-Ingredient");
		for (int i = 0; i < array.Length; i++)
		{
			WorkableItem component = array[i].GetComponent<WorkableItem>();
			if ((bool)component)
			{
				GameObject nextPrefab = component.GetNextPrefab();
				IngredientPropertiesComponent ingredientPropertiesComponent = nextPrefab.RequireComponent<IngredientPropertiesComponent>();
				AssembledDefinitionNode orderComposition = ingredientPropertiesComponent.GetOrderComposition();
				if (AssembledDefinitionNode.Matching(_orderToBe, orderComposition))
				{
					list.Add(array[i]);
				}
			}
		}
		return list.ToArray();
	}

	public static int GetWorkableChopsPerSlice()
	{
		int result = 1;
		int count = ClientUserSystem.m_Users.Count;
		GameSession gameSession = GetGameSession();
		switch (gameSession.TypeSettings.Type)
		{
		case GameSession.GameType.Cooperative:
			result = ((count != 1) ? 1 : GetGameConfig().SingleplayerChopTimeMultiplier);
			break;
		case GameSession.GameType.Competitive:
			result = ((count >= 4) ? 1 : GetGameConfig().SingleplayerChopTimeMultiplier);
			break;
		}
		return result;
	}

	private static EngagementSlot[] GetEngagedAndInGame()
	{
		EngagementSlot[] _array = new EngagementSlot[0];
		PlayerManager playerManager = RequireManager<PlayerManager>();
		OvercookedEngagementController overcookedEngagementController = RequireManager<OvercookedEngagementController>();
		bool flag = overcookedEngagementController.GlobalLevelType == OvercookedEngagementController.LevelType.WithChefs;
		for (int i = 0; i < 4; i++)
		{
			EngagementSlot engagementSlot = (EngagementSlot)i;
			GamepadUser user = playerManager.GetUser(engagementSlot);
			if (user != null && (user.StickyEngagement || !flag))
			{
				ArrayUtils.PushBack(ref _array, engagementSlot);
			}
		}
		return _array;
	}

	public static void FireDeedOnAllChefs<T>(params object[] _params) where T : DeedManagerBase.PadDeed
	{
		DeedManager deedManager = RequireManager<DeedManager>();
		EngagementSlot[] engagedAndInGame = GetEngagedAndInGame();
		for (int i = 0; i < engagedAndInGame.Length; i++)
		{
			FireDeed<T>((ControlPadInput.PadNum)engagedAndInGame[i], _params);
		}
	}

	public static void FireDeed<T>(ControlPadInput.PadNum _player, params object[] _params) where T : DeedManagerBase.Deed
	{
		DeedManager deedManager = RequireManager<DeedManager>();
		Type[] array = new Type[_params.Length + 1];
		array[0] = typeof(ControlPadInput.PadNum);
		for (int i = 0; i < _params.Length; i++)
		{
			array[i + 1] = _params[i].GetType();
		}
		ConstructorInfo constructor = typeof(T).GetConstructor(array);
		object[] array2 = new object[_params.Length + 1];
		array2[0] = _player;
		_params.CopyTo(array2, 1);
		object obj = constructor.Invoke(null, array2);
		deedManager.FireDeed(obj as T);
	}

	public static GameObject[] GetIngredients(OrderDefinitionNode _orderToBe)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Ingredient");
		for (int i = 0; i < array.Length; i++)
		{
			IngredientPropertiesComponent ingredientPropertiesComponent = array[i].RequireComponent<IngredientPropertiesComponent>();
			AssembledDefinitionNode orderComposition = ingredientPropertiesComponent.GetOrderComposition();
			if (AssembledDefinitionNode.Matching(_orderToBe, orderComposition))
			{
				list.Add(array[i]);
			}
		}
		return list.ToArray();
	}

	public static GameObject[] GetIngredientCrates(OrderDefinitionNode _node)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Crate");
		Predicate<GameObject> match = delegate(GameObject _crate)
		{
			ClientPickupItemSpawner clientPickupItemSpawner = _crate.RequireComponent<ClientPickupItemSpawner>();
			GameObject itemPrefab = clientPickupItemSpawner.GetItemPrefab();
			WorkableItem component = itemPrefab.GetComponent<WorkableItem>();
			if (component != null)
			{
				GameObject nextPrefab = component.GetNextPrefab();
				IngredientPropertiesComponent component2 = nextPrefab.GetComponent<IngredientPropertiesComponent>();
				if ((bool)component2)
				{
					return AssembledDefinitionNode.Matching(_node, component2.GetOrderComposition());
				}
			}
			return false;
		};
		return array.FindAll(match);
	}

	public static GameObject[] FindContainersWithSubset(string _tag, AssembledDefinitionNode[] _contents, GameObject[] _containers = null)
	{
		GameObject[] array = _containers;
		if (array == null)
		{
			array = GameObject.FindGameObjectsWithTag(_tag);
		}
		Predicate<GameObject> match = delegate(GameObject _obj)
		{
			if (_obj == null || !_obj.CompareTag(_tag) || !_obj.gameObject.activeInHierarchy)
			{
				return false;
			}
			IIngredientContents ingredientContents = _obj.RequestInterface<IIngredientContents>();
			if (ingredientContents == null)
			{
				return false;
			}
			AssembledDefinitionNode[] contents = ingredientContents.GetContents();
			bool[] array2 = new bool[contents.Length];
			for (int i = 0; i < _contents.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < contents.Length; j++)
				{
					if (!array2[j] && AssembledDefinitionNode.Matching(_contents[i], contents[j]))
					{
						flag = true;
						array2[j] = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		};
		return array.FindAll(match);
	}

	public static GameObject[] FindEmptyContainers(string _tag, GameObject[] _containers = null)
	{
		GameObject[] array = _containers;
		if (array == null)
		{
			array = GameObject.FindGameObjectsWithTag(_tag);
		}
		Predicate<GameObject> match = delegate(GameObject _obj)
		{
			if (_obj == null || !_obj.CompareTag(_tag) || !_obj.gameObject.activeInHierarchy)
			{
				return false;
			}
			IIngredientContents ingredientContents = _obj.RequestInterface<IIngredientContents>();
			return ingredientContents != null && ingredientContents.GetContents().Length == 0;
		};
		return array.FindAll(match);
	}

	public static GameObject[] GetPlayerHeldItems()
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			ICarrier carrier = array[i].RequireInterface<ICarrier>();
			GameObject gameObject = carrier.InspectCarriedItem();
			if (gameObject != null)
			{
				list.Add(gameObject);
			}
		}
		return list.ToArray();
	}

	public static void QuitLevel()
	{
		GameSession gameSession = GetGameSession();
		if (gameSession == null)
		{
			ServerGameSetup.Mode = GameMode.OnlineKitchen;
			ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
			return;
		}
		GameSession.GameTypeSettings typeSettings = gameSession.TypeSettings;
		if (ConnectionStatus.IsHost())
		{
			if (SceneManager.GetActiveScene().name == typeSettings.WorldMapScene)
			{
				ServerGameSetup.Mode = GameMode.OnlineKitchen;
				ServerOptions serverOptions = (ServerOptions)ConnectionModeSwitcher.GetAgentData();
				serverOptions.gameMode = GameMode.OnlineKitchen;
				serverOptions.visibility = OnlineMultiplayerSessionVisibility.ePrivate;
				ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Server, serverOptions);
				ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
				return;
			}
			switch (ClientGameSetup.Mode)
			{
			case GameMode.Campaign:
			{
				GameProgress progress = gameSession.Progress;
				if (progress.LoadFirstScene && !progress.SaveData.IsLevelComplete(gameSession.Progress.FirstSceneIndex) && !DebugManager.Instance.GetOption("Unlock all levels"))
				{
					ServerGameSetup.Mode = GameMode.OnlineKitchen;
					ServerOptions serverOptions3 = (ServerOptions)ConnectionModeSwitcher.GetAgentData();
					serverOptions3.gameMode = GameMode.OnlineKitchen;
					serverOptions3.visibility = OnlineMultiplayerSessionVisibility.ePrivate;
					ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Server, serverOptions3);
					ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
				}
				else
				{
					gameSession.FillShownMetaDialogStatus();
					ServerMessenger.GameProgressData(gameSession.Progress.SaveData, gameSession.m_shownMetaDialogs);
					ServerMessenger.LoadLevel(typeSettings.WorldMapScene, GameState.CampaignMap, true, GameState.RunMapUnfoldRoutine);
				}
				break;
			}
			case GameMode.Party:
			case GameMode.Versus:
			{
				ServerUserSystem.RemoveMatchmadeUsers();
				bool flag = false;
				for (int i = 0; i < ServerUserSystem.m_Users.Count; i++)
				{
					if (!ServerUserSystem.m_Users._items[i].IsLocal)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					ServerGameSetup.Mode = GameMode.OnlineKitchen;
					ServerOptions serverOptions2 = (ServerOptions)ConnectionModeSwitcher.GetAgentData();
					serverOptions2.gameMode = GameMode.OnlineKitchen;
					serverOptions2.visibility = OnlineMultiplayerSessionVisibility.ePrivate;
					ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Server, serverOptions2);
				}
				else
				{
					ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Offline, null, OnQuitLevelOfflineRequestResult);
				}
				ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
				break;
			}
			}
			return;
		}
		if (ConnectionStatus.IsInSession())
		{
			ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Offline);
			LoadingScreenFlow.LoadScene("StartScreen");
			return;
		}
		if (SceneManager.GetActiveScene().name == typeSettings.WorldMapScene)
		{
			ServerGameSetup.Mode = GameMode.OnlineKitchen;
			ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
			return;
		}
		switch (ClientGameSetup.Mode)
		{
		case GameMode.Campaign:
		{
			GameProgress progress2 = gameSession.Progress;
			if (progress2.LoadFirstScene && !progress2.SaveData.IsLevelComplete(gameSession.Progress.FirstSceneIndex) && !DebugManager.Instance.GetOption("Unlock all levels"))
			{
				ServerGameSetup.Mode = GameMode.OnlineKitchen;
				ServerMessenger.LoadLevel("StartScreen", GameState.MainMenu, true);
			}
			else
			{
				ServerMessenger.LoadLevel(typeSettings.WorldMapScene, GameState.CampaignMap, true, GameState.RunMapUnfoldRoutine);
			}
			break;
		}
		case GameMode.Party:
			ServerMessenger.LoadLevel(typeSettings.WorldMapScene, GameState.PartyLobby, true);
			break;
		case GameMode.Versus:
			ServerMessenger.LoadLevel(typeSettings.WorldMapScene, GameState.VSLobby, true);
			break;
		}
	}

	public static void OnQuitLevelOfflineRequestResult(IConnectionModeSwitchStatus _status)
	{
		if (_status.GetResult() == eConnectionModeSwitchResult.Success)
		{
			ServerGameSetup.Mode = GameMode.OnlineKitchen;
		}
		else if (_status.GetResult() == eConnectionModeSwitchResult.Failure)
		{
			ConnectionModeSwitcher.RequestConnectionState(NetConnectionState.Offline, new OfflineOptions
			{
				hostUser = RequireManager<PlayerManager>().GetUser(EngagementSlot.One),
				connectionMode = OnlineMultiplayerConnectionMode.eNone
			}, OnQuitLevelFullOfflineRequestResult);
		}
	}

	public static void OnQuitLevelFullOfflineRequestResult(IConnectionModeSwitchStatus _status)
	{
		if (_status.GetResult() == eConnectionModeSwitchResult.Success)
		{
			ServerGameSetup.Mode = GameMode.OnlineKitchen;
		}
	}

	public static void LoadLevel(string _levelName)
	{
		LoadScene(_levelName);
	}

	public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		if (CanLoadScene(sceneName))
		{
			if (mode == LoadSceneMode.Single)
			{
				ScreenTransitionManager screenTransitionManager = RequireManager<ScreenTransitionManager>();
				screenTransitionManager.TransitionLoad(sceneName);
			}
			else
			{
				AssetBundleManager.LoadLevel(sceneName.ToLowerInvariant(), sceneName, mode == LoadSceneMode.Additive);
			}
		}
	}

	public static bool CanLoadScene(string sceneName)
	{
		return true;
	}

	public static int GetRequiredBitCount(int value)
	{
		return (int)(Math.Log(value, 2.0) + 1.0);
	}

	public static void InitialiseAnalytics(string serverUrl, string gameKey, string secretKey, string userId, string buildVersion)
	{
		if (s_AnalyticsHelper == null)
		{
			DeviceInfo.PlatformEnum platformEnum = DeviceInfo.PlatformEnum.NOT_SET;
			platformEnum = DeviceInfo.PlatformEnum.STEAM;
			s_AnalyticsHelper = new Helper(serverUrl, gameKey, secretKey, platformEnum, userId, buildVersion);
			s_AnalyticsHelper.EnableAnalytics(s_AnalyticsEnabled);
		}
	}

	public static void ShutdownAnalytics()
	{
		s_AnalyticsHelper.EnableAnalytics(false);
		s_AnalyticsHelper = null;
	}

	public static void SendDiagnosticEvent(string eventName)
	{
		if (GetT17AnalyticsHelper() != null)
		{
			GetT17AnalyticsHelper().AddDesignEvent(eventName, null);
		}
	}

	public static void SendDiagnosticValueEvent(string eventName, float value)
	{
		if (GetT17AnalyticsHelper() != null)
		{
			GetT17AnalyticsHelper().AddDesignEvent(eventName, value);
		}
	}
}
