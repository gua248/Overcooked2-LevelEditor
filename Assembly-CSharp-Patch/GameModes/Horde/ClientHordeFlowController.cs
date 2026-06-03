#define ANALYTICS
using System;
using System.Collections;
using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameModes.Horde
{
	public class ClientHordeFlowController : ClientFlowControllerBase, IRecipeListCache
	{
		private HordeFlowController m_flowController;

		private HordeLevelConfig m_levelConfig;

		private ClientHordeTarget[] m_targets;

		private ClientHordeEnemy[] m_enemies;

		private RecipeList.Entry[] m_entries;

		private TeamScoreStats m_score = default(TeamScoreStats);

		private IEnumerator m_runWaveIntro;

		private IEnumerator m_runWaveOutro;

		private GenericVoid<RecipeList.Entry>[] m_onEntryAddedForTarget;

		private GenericVoid<RecipeList.Entry>[] m_onSuccessfulDeliveryForTarget;

		private GenericVoid<RecipeList.Entry>[] m_onIncorrectDeliveryForTarget;

		private GenericVoid<ClientHordeEnemy>[] m_onEnemyApproachingTarget;

		private GenericVoid<ClientHordeEnemy>[] m_onEnemyDespawningForTarget;

		private GenericVoid<ClientHordeFlowController, int> m_onBeginWave;

		private GenericVoid<int> m_onMoneyChanged;

		private List<OrderDefinitionNode> m_cachedRecipeList = new List<OrderDefinitionNode>(8);

		private List<AssembledDefinitionNode> m_cachedAssembledRecipes = new List<AssembledDefinitionNode>(8);

		private List<CookingStepData> m_cachedCookingStepList = new List<CookingStepData>(8);

		public int MaxHealth { get; private set; }

		public int Health
		{
			get
			{
				return m_score.TotalHealth;
			}
		}

		public int Money
		{
			get
			{
				return m_score.GetTotalMoney();
			}
		}

		public void RegisterOnSuccessfulDelivery(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onSuccessfulDelivery)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onSuccessfulDeliveryForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Combine(m_onSuccessfulDeliveryForTarget[num], onSuccessfulDelivery);
		}

		public void UnregisterOnSuccessfulDelivery(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onSuccessfulDelivery)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onSuccessfulDeliveryForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Remove(m_onSuccessfulDeliveryForTarget[num], onSuccessfulDelivery);
		}

		public void RegisterOnIncorrectDelivery(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onIncorrectDelivery)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onIncorrectDeliveryForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Combine(m_onIncorrectDeliveryForTarget[num], onIncorrectDelivery);
		}

		public void UnregisterOnIncorrectDelivery(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onIncorrectDelivery)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onIncorrectDeliveryForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Remove(m_onIncorrectDeliveryForTarget[num], onIncorrectDelivery);
		}

		public void RegisterOnEntryAdded(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onOrderChanged)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEntryAddedForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Combine(m_onEntryAddedForTarget[num], onOrderChanged);
		}

		public void UnregisterOnEntryAdded(object handle, ClientHordeTarget target, GenericVoid<RecipeList.Entry> onOrderChanged)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEntryAddedForTarget)[num] = (GenericVoid<RecipeList.Entry>)Delegate.Remove(m_onEntryAddedForTarget[num], onOrderChanged);
		}

		public void RegisterOnEnemyApproaching(object handle, ClientHordeTarget target, GenericVoid<ClientHordeEnemy> onEnemyApproaching)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEnemyApproachingTarget)[num] = (GenericVoid<ClientHordeEnemy>)Delegate.Combine(m_onEnemyApproachingTarget[num], onEnemyApproaching);
		}

		public void UnregisterOnEnemyApproaching(object handle, ClientHordeTarget target, GenericVoid<ClientHordeEnemy> onEnemyApproaching)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEnemyApproachingTarget)[num] = (GenericVoid<ClientHordeEnemy>)Delegate.Remove(m_onEnemyApproachingTarget[num], onEnemyApproaching);
		}

		public void RegisterOnEnemyDespawning(object handle, ClientHordeTarget target, GenericVoid<ClientHordeEnemy> onEnemyDespawning)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEnemyDespawningForTarget)[num] = (GenericVoid<ClientHordeEnemy>)Delegate.Combine(m_onEnemyDespawningForTarget[num], onEnemyDespawning);
		}

		public void UnregisterOnEnemyDeath(object handle, ClientHordeTarget target, GenericVoid<ClientHordeEnemy> onEnemyDespawning)
		{
			int num = Array.IndexOf(m_targets, target);
			(m_onEnemyDespawningForTarget)[num] = (GenericVoid<ClientHordeEnemy>)Delegate.Remove(m_onEnemyDespawningForTarget[num], onEnemyDespawning);
		}

		public void RegisterOnBeginWave(object handle, GenericVoid<ClientHordeFlowController, int> onBeginWave)
		{
			m_onBeginWave = (GenericVoid<ClientHordeFlowController, int>)Delegate.Combine(m_onBeginWave, onBeginWave);
		}

		public void UnregisterOnBeginWave(object handle, GenericVoid<ClientHordeFlowController, int> onBeginWave)
		{
			m_onBeginWave = (GenericVoid<ClientHordeFlowController, int>)Delegate.Remove(m_onBeginWave, onBeginWave);
		}

		public void RegisterOnMoneyChanged(object handle, GenericVoid<int> onMoneyChanged)
		{
			m_onMoneyChanged = (GenericVoid<int>)Delegate.Combine(m_onMoneyChanged, onMoneyChanged);
		}

		public void UnregisterOnMoneyChanged(object handle, GenericVoid<int> onMoneyChanged)
		{
			m_onMoneyChanged = (GenericVoid<int>)Delegate.Remove(m_onMoneyChanged, onMoneyChanged);
		}

		public override EntityType GetEntityType()
		{
			return EntityType.HordeFlowController;
		}

		public override void StartSynchronising(Component synchronisedObject)
		{
			base.StartSynchronising(synchronisedObject);
			CacheRecipeListData();
			m_flowController = (HordeFlowController)synchronisedObject;
			m_levelConfig = GameUtils.GetLevelConfig() as HordeLevelConfig;
			GameUtils.InstantiateUIControllerOnScalingHUDCanvas(m_flowController.m_uiPrefab);
			MaxHealth = m_levelConfig.m_health;
			m_score.TotalHealth = MaxHealth;
			List<GameObject> list = new List<GameObject>();
			SceneManager.GetActiveScene().GetRootGameObjects(list);
			List<ClientHordeTarget> list2 = new List<ClientHordeTarget>(8);
			for (int i = 0; i < list.Count; i++)
			{
				list2.AddRange(list[i].RequestComponentsRecursive<ClientHordeTarget>());
			}
			list2.Sort(default(ClientHordeTargetComparer));
			m_targets = list2.ToArray();
			m_enemies = new ClientHordeEnemy[m_targets.Length];
			m_entries = new RecipeList.Entry[m_targets.Length];
			m_onSuccessfulDeliveryForTarget = new GenericVoid<RecipeList.Entry>[m_targets.Length];
			m_onIncorrectDeliveryForTarget = new GenericVoid<RecipeList.Entry>[m_targets.Length];
			m_onEntryAddedForTarget = new GenericVoid<RecipeList.Entry>[m_targets.Length];
			m_onEnemyApproachingTarget = new GenericVoid<ClientHordeEnemy>[m_targets.Length];
			m_onEnemyDespawningForTarget = new GenericVoid<ClientHordeEnemy>[m_targets.Length];
			for (int j = 0; j < m_levelConfig.m_waves.Count; j++)
			{
				HordeWaveData hordeWaveData = m_levelConfig.m_waves[j];
				for (int k = 0; k < hordeWaveData.m_spawns.Count; k++)
				{
					HordeSpawnData hordeSpawnData = hordeWaveData.m_spawns[k];
					for (int l = 0; l < m_targets.Length; l++)
					{
						NetworkUtils.RegisterSpawnablePrefab(m_targets[l].gameObject, hordeSpawnData.m_prefab);
					}
				}
			}
		}

		private IEnumerator RunWaveIntro(GameObject waveNumberUIPrefab, float waveNumberUIDelay, string waveNumberUILocalisationTag, HordeWavesData waves, int waveIndex)
		{
			GameObject waveNumberUIInstance = GameUtils.InstantiateUIControllerOnScalingHUDCanvas(waveNumberUIPrefab);
			T17Text waveNumberText = waveNumberUIInstance.RequireComponentRecursive<T17Text>();
			waveNumberUIInstance.SetActive(false);
			string waveNumberLocalisedText = Localization.Get(waveNumberUILocalisationTag, new LocToken("[Number]", (waveIndex + 1).ToString()), new LocToken("[NumberMax]", waves.Count.ToString()));
			waveNumberText.SetNonLocalizedText(waveNumberLocalisedText);
			waveNumberUIInstance.SetActive(true);
			GameUtils.TriggerAudio(GameOneShotAudioTag.DLC_07_Wave_Incoming, waveNumberUIInstance.layer);
			m_onBeginWave(this, waveIndex + 1);
			int layerId = LayerMask.NameToLayer("Default");
			IEnumerator timerRoutine = CoroutineUtils.TimerRoutine(waveNumberUIDelay, layerId);
			while (timerRoutine.MoveNext())
			{
				yield return null;
			}
			waveNumberUIInstance.SetActive(false);
			UnityEngine.Object.Destroy(waveNumberUIInstance);
			yield return null;
		}

		private IEnumerator RunWaveOutro(HordeWavesData waves, int index)
		{
			IEnumerator timer = CoroutineUtils.TimerRoutine(waves[index].m_intervalSeconds, base.gameObject.layer);
			while (timer != null && timer.MoveNext())
			{
				yield return null;
			}
		}

		public override void UpdateSynchronising()
		{
			base.UpdateSynchronising();
			if (m_runWaveIntro != null && !m_runWaveIntro.MoveNext())
			{
				m_runWaveIntro = null;
			}
			if (m_runWaveOutro != null && !m_runWaveOutro.MoveNext())
			{
				m_runWaveOutro = null;
			}
		}

		public override void ApplyServerEvent(Serialisable serialisable)
		{
			HordeFlowMessage hordeFlowMessage = (HordeFlowMessage)(object)serialisable;
			switch (hordeFlowMessage.m_kind)
			{
			case HordeFlowMessage.Kind.BeginWave:
				m_score.Copy(hordeFlowMessage.m_score);
				m_onMoneyChanged(m_score.GetTotalMoney());
				m_runWaveIntro = RunWaveIntro(m_flowController.m_waveNumberUIPrefab, m_flowController.m_waveNumberUIDelay, m_flowController.m_waveNumberUILocalisationTag, m_levelConfig.m_waves, hordeFlowMessage.m_index);
				break;
			case HordeFlowMessage.Kind.EndWave:
				m_score.Copy(hordeFlowMessage.m_score);
				m_onMoneyChanged(m_score.GetTotalMoney());
				m_runWaveOutro = RunWaveOutro(m_levelConfig.m_waves, hordeFlowMessage.m_index);
				break;
			case HordeFlowMessage.Kind.Spawn:
			{
				ClientHordeTarget target = m_targets[hordeFlowMessage.m_index];
				ClientHordeEnemy clientHordeEnemy = hordeFlowMessage.m_enemy.RequireComponent<ClientHordeEnemy>();
				clientHordeEnemy.Setup(this, target);
				clientHordeEnemy.RegisterOnBeginState(this, OnEnemyBeginState);
				m_enemies[hordeFlowMessage.m_index] = clientHordeEnemy;
				break;
			}
			case HordeFlowMessage.Kind.EntryAdded:
			{
				m_entries[hordeFlowMessage.m_index] = hordeFlowMessage.m_entry;
				RecipeList.Entry param3 = m_entries[hordeFlowMessage.m_index];
				m_onEntryAddedForTarget[hordeFlowMessage.m_index](param3);
				break;
			}
			case HordeFlowMessage.Kind.SuccessfulDelivery:
			{
				m_score.Copy(hordeFlowMessage.m_score);
				m_onMoneyChanged(m_score.GetTotalMoney());
				RecipeList.Entry param2 = m_entries[hordeFlowMessage.m_index];
				m_entries[hordeFlowMessage.m_index] = null;
				m_onSuccessfulDeliveryForTarget[hordeFlowMessage.m_index](param2);
				GameUtils.TriggerAudio(GameOneShotAudioTag.SuccessfulDelivery, base.gameObject.layer);
				break;
			}
			case HordeFlowMessage.Kind.IncorrectDelivery:
			{
				m_score.Copy(hordeFlowMessage.m_score);
				m_onMoneyChanged(m_score.GetTotalMoney());
				RecipeList.Entry param = m_entries[hordeFlowMessage.m_index];
				m_onIncorrectDeliveryForTarget[hordeFlowMessage.m_index](param);
				GameUtils.TriggerAudio(GameOneShotAudioTag.RecipeTimeOut, base.gameObject.layer);
				break;
			}
			case HordeFlowMessage.Kind.ScoreOnly:
				m_score.Copy(hordeFlowMessage.m_score);
				m_onMoneyChanged(m_score.GetTotalMoney());
				break;
			}
		}

		private void OnEnemyBeginState(ClientHordeEnemy enemy, HordeEnemyBehaviorState fromState, HordeEnemyBehaviorState state)
		{
			switch (state)
			{
			case HordeEnemyBehaviorState.Move:
			{
				int num2 = Array.IndexOf(m_enemies, enemy);
				m_onEnemyApproachingTarget[num2](enemy);
				break;
			}
			case HordeEnemyBehaviorState.Despawn:
			{
				int num = Array.IndexOf(m_enemies, enemy);
				m_onEnemyDespawningForTarget[num](enemy);
				break;
			}
			}
		}

		private void OnLevelRestartRequested()
		{
			bool flag = !ConnectionStatus.IsHost() && ConnectionStatus.IsInSession();
			bool flag2 = ClientGameSetup.Mode != GameMode.Campaign;
			if (!flag && !flag2)
			{
				base.gameObject.RequireComponent<ServerHordeFlowController>().OnLevelRestartRequested();
			}
		}

		protected override IEnumerator RunLevelOutro()
		{
			int levelID = GameUtils.GetLevelID();
			OvercookedAchievementManager overcookedAchievementManager = GameUtils.RequestManager<OvercookedAchievementManager>();
			if (overcookedAchievementManager != null && Health >= MaxHealth)
			{
				overcookedAchievementManager.AddIDStat(700, levelID, ControlPadInput.PadNum.One);
			}
			int num = 3;
			int starRating = 0;
			if (Health > 0 && Health < MaxHealth)
			{
				starRating = 1 + (int)Mathf.Round(MathUtils.ClampedRemap(Health, 0f, MaxHealth, -0.49f, (float)(num - 2) - 0.51f));
			}
			else if (Health >= MaxHealth)
			{
				starRating = num;
			}
			int requiredBitCount = GameUtils.GetRequiredBitCount(65535);
			GameSession gameSession = GameUtils.GetGameSession();
			GameProgress.UnlockData[] _unlocks = new GameProgress.UnlockData[0];
			gameSession.Progress.RecordLevelProgress(levelID, starRating, ref _unlocks);
			gameSession.Progress.RecordLevelScore(new GameProgress.HighScores.Score
			{
				iLevelID = levelID,
				iHighScore = FloatUtils.ToUnorm((float)Health / (float)MaxHealth, requiredBitCount)
			});
			if (ConnectionStatus.IsHost() || !ConnectionStatus.IsInSession())
			{
				Analytics.LogEvent("Level Score", Health, Analytics.Flags.LevelName | Analytics.Flags.PlayerCount);
			}
			HordeOutroFlowroutine hordeOutroFlowroutine = new HordeOutroFlowroutine();
			HordeRatingUIController.ScoreData scoreData = new HordeRatingUIController.ScoreData
			{
				m_health = (float)m_score.TotalHealth / (float)MaxHealth,
				m_moneyEarned = m_score.TotalMoneyEarned,
				m_enemiesDefeated = m_score.TotalEnemiesDefeated
			};
			m_levelConfig.m_flowroutineData.m_scoreData = scoreData;
			m_levelConfig.m_flowroutineData.m_health = m_score.TotalHealth;
			m_levelConfig.m_flowroutineData.m_success = (float)m_score.TotalHealth > 0f;
			m_levelConfig.m_flowroutineData.m_unlocks = _unlocks;
			hordeOutroFlowroutine.OnRestartRequest = OnLevelRestartRequested;
			return hordeOutroFlowroutine.BuildFlowroutine(m_levelConfig.m_flowroutineData);
		}

		public OrderDefinitionNode[] GetCachedRecipeList()
		{
			return m_cachedRecipeList.ToArray();
		}

		public AssembledDefinitionNode[] GetCachedAssembledRecipes()
		{
			return m_cachedAssembledRecipes.ToArray();
		}

		public CookingStepData[] GetCachedCookingSteps()
		{
			return m_cachedCookingStepList.ToArray();
		}

		private void CacheRecipeListData()
		{
			LevelConfigBase levelConfig = GetLevelConfig();
			if (levelConfig != null && levelConfig.m_recipeMatchingList != null)
			{
				m_cachedRecipeList.Capacity = levelConfig.m_recipeMatchingList.m_recipes.Length;
				m_cachedCookingStepList.Capacity = levelConfig.m_recipeMatchingList.m_cookingSteps.Length;
				m_cachedRecipeList.AddRange(levelConfig.m_recipeMatchingList.m_recipes);
				m_cachedCookingStepList.AddRange(levelConfig.m_recipeMatchingList.m_cookingSteps);
				for (int i = 0; i < levelConfig.m_recipeMatchingList.m_includeLists.Length; i++)
				{
					m_cachedRecipeList.AddRange(levelConfig.m_recipeMatchingList.m_includeLists[i].m_recipes);
					m_cachedCookingStepList.AddRange(levelConfig.m_recipeMatchingList.m_includeLists[i].m_cookingSteps);
				}
				m_cachedAssembledRecipes.Capacity = m_cachedRecipeList.Count;
				for (int j = 0; j < m_cachedRecipeList.Count; j++)
				{
					m_cachedAssembledRecipes.Add(m_cachedRecipeList[j].Convert().Simpilfy());
				}
			}
		}
	}
}
