using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
    public enum GameEditState
    {
        Edit,
        PrepareForBuilding,
        Game,
    }

    [ExecuteInEditMode]
    [DefaultExecutionOrder(-100)]
    public class PseudoPrefabManager : MonoBehaviour
    {

        public PseudoPrefabManagerStub stub;
        public static PseudoPrefabManager Instance;

        private Dictionary<string, AssetBundle> bundleDict = new Dictionary<string, AssetBundle>();
        private AssetBundleManifest assetBundleManifest = null;

        public Dictionary<string, Material> editedMaterials = new Dictionary<string, Material>();

        public bool prepareForBuilding;

        public GameEditState GameEditState
        {
            get
            {
                if (Application.isPlaying) return GameEditState.Game;
                else if (prepareForBuilding) return GameEditState.PrepareForBuilding;
                else return GameEditState.Edit;
            }
        }

        private void Awake()
        {
            stub = GetComponent<PseudoPrefabManagerStub>();
        }

        private void OnEnable()
        {
            if (Instance != null && Instance.gameObject != this.gameObject)
            {
                Instance.gameObject.Destroy();
            }
            Instance = this;

            if (assetBundleManifest == null)
                Init();
        }

        // weird
        // when exiting play mode in editor,
        // assetBundleManifest != null after OnEnable() but
        // assetBundleManifest == null before Start()
        private void Start()
        {
            if (assetBundleManifest == null)
                Init();
        }

        private void OnDisable()
        {
            DeInit();
            Instance = null;
        }

        public void Init()
        {
            EnsureLoadAllAssetBundles();
            if (GameEditState == GameEditState.Game || GameEditState == GameEditState.Edit)
            {
                SetAssetRef();

                if (GameEditState == GameEditState.Game)
                {
                    GameSession gameSession = GameUtils.GetGameSession();
                    stub.BootstrapManagerGO.GetComponent<KitchenBootstrapManager>().EnsureSetup();
                    if (gameSession == null)
                    {
                        GameSession gameSession1 = GameUtils.GetGameSession();
                        typeof(GameProgress)
                            .GetField("m_sceneDirectory", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                            .SetValue(gameSession1.Progress, LevelConfigSetup.SetupSceneDirectoryData(stub.configTemplateSO, stub.levelInfo));
                        var sceneDirectoryVarientEntry = gameSession1.Progress.GetSceneDirectory().Scenes[0].GetSceneVarient(4);
                        gameSession1.LevelSettings = new GameSession.GameLevelSettings() { SceneDirectoryVarientEntry = sceneDirectoryVarientEntry };
                    }
                }

                editedMaterials.Clear();
                ResetAllPseudoPrefabs();
            }
        }

        public void DeInit()
        {
            UnSetAssetRef();
            ClearAllPseudoPrefabs();
            foreach (var key in bundleDict.Keys.ToArray())
            {
                UnloadAssetBundle(key);
            }
            bundleDict.Clear();
            editedMaterials.Clear();
            assetBundleManifest = null;
        }

        private void EnsureLoadAllAssetBundles()
        {
            if (assetBundleManifest != null) return;
            string manifestAssetBundleName = "Windows";
            var bundle = LoadAssetBundle(manifestAssetBundleName, true);
            assetBundleManifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            foreach (string name in stub.levelInfo.dependencies)
            {
                LoadAssetBundle(name, false);
            }
            Debug.Log("All loaded bundles: " + string.Join(" ", bundleDict.Select(x => x.Key).ToArray()));
        }

        // set all prefab ref in the scene
        private void SetAssetRef()
        {
            Component component = stub.FlowManagerGO.GetComponent<LevelIntroFlowroutine>();
            LevelIntroFlowroutineData m_data = (LevelIntroFlowroutineData)component.GetType()
                .GetField("m_data", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(component);
            m_data.GoUIPrefab = LoadAsset(stub.GoSO);
            m_data.ReadyUIPrefab = LoadAsset(stub.ReadySO);
            m_data.TutorialPopup.Prefab = LoadAsset(stub.TutorialSplashSO);

            component = stub.RecipeUIGO.GetComponent<RecipeFlowGUI>();
            component.GetType()
                .GetField("m_recipeWidgetPrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset(stub.RecipeUISO).GetComponent<RecipeWidgetUIController>());

            component = stub.AudioManagerGO.GetComponent<CampaignAudioManager>();
            component.GetType()
                .GetField("m_inLevelMusic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset<AudioClip>(stub.InLevelMusicSO));
            component.GetType()
                .GetField("m_inLevelAmbiences", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, stub.InLevelAmbiences.Select(x => (GameLoopingAudioTag)x).ToArray());
            component.GetType()
                .GetField("m_summaryScreenMusic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset<AudioClip>(stub.RoundResultsSO));
            AudioDirectoryData[] m_audioDirectories = new AudioDirectoryData[stub.AudioDirectorySOs.Length];
            for (int i = 0; i < stub.AudioDirectorySOs.Length; i++)
            {
                m_audioDirectories[i] = LoadAsset<AudioDirectoryData>(stub.AudioDirectorySOs[i]);
                foreach (var audio in m_audioDirectories[i].LoopingAudio)
                    if (audio.Tag == GameLoopingAudioTag.Flamethrower)
                        audio.Volume = 0.5f;
            }
            typeof(AudioManager)
                .GetField("m_audioDirectories", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, m_audioDirectories);

            component = stub.PlayerSwitchingManagerGO.GetComponent<PlayerSwitchingManager>();
            component.GetType()
                .GetField("m_transitionParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset(stub.PFXSOs[0]).GetComponent<ParticleSystem>());
            component.GetType()
                .GetField("m_transitionStartParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset(stub.PFXSOs[1]).GetComponent<ParticleSystem>());
            component.GetType()
                .GetField("m_transitionEndParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset(stub.PFXSOs[2]).GetComponent<ParticleSystem>());

            component = stub.BootstrapManagerGO.GetComponent<KitchenBootstrapManager>();
            ChefAvatarData chefAvatarData = LoadAsset<ChefAvatarData>(stub.PlayerBlackCatSO);
            ChefColourData[] chefColourData = stub.PlayerColourSOs.Select(x => LoadAsset<ChefColourData>(x)).ToArray();
            component.GetType()
                .GetField("m_playerOneChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(chefAvatarData, chefColourData[0]));
            component.GetType()
                .GetField("m_playerTwoChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(chefAvatarData, chefColourData[1]));
            component.GetType()
                .GetField("m_playerThreeChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(chefAvatarData, chefColourData[2]));
            component.GetType()
                .GetField("m_playerFourChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(chefAvatarData, chefColourData[3]));
            typeof(BootstrapManager)
                .GetField("m_gameMetaEnvironmentPrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LoadAsset(stub.GameMetaEnvironmentSO));
            component.GetType()
                .GetField("m_bootstrapConfig", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, LevelConfigSetup.SetupConfig(stub.configTemplateSO, stub.levelInfo, 4));

            component = stub.KillPlaneGO.GetComponent<RespawnCollider>();
            if (stub.OnDeathEffectSO != null)
                (component as RespawnCollider).m_onDeathEffect = LoadAsset(stub.OnDeathEffectSO);
        }

        public static void ResetAllPseudoPrefabs()
        {
            PseudoPrefab[] pseudoPrefabs = GameObject.FindObjectsOfType<PseudoPrefab>();
            foreach (var pseudoPrefab in pseudoPrefabs)
                pseudoPrefab.ResetChild();
            foreach (var pseudoPrefab in pseudoPrefabs)
                pseudoPrefab.LateSetup();

            PseudoParticleSystem[] pseudoParticleSystems = GameObject.FindObjectsOfType<PseudoParticleSystem>();
            foreach (var pseudoParticleSystem in pseudoParticleSystems)
                pseudoParticleSystem.Setup();
        }

        public static void ClearAllPseudoPrefabs()
        {
            PseudoPrefab[] pseudoPrefabs = GameObject.FindObjectsOfType<PseudoPrefab>();
            foreach (var pseudoPrefab in pseudoPrefabs)
                pseudoPrefab.ClearChild();
            PseudoParticleSystem[] pseudoParticleSystems = GameObject.FindObjectsOfType<PseudoParticleSystem>();
            foreach (var pseudoParticleSystem in pseudoParticleSystems)
                pseudoParticleSystem.Clear();
        }

        public static void SetupAfterStartSynchronisingAllPseudoPrefabs()
        {
            PseudoPrefab[] pseudoPrefabs = GameObject.FindObjectsOfType<PseudoPrefab>();
            foreach (var pseudoPrefab in pseudoPrefabs)
                pseudoPrefab.SetupAfterStartSynchronising();
        }

        // unset all prefab ref in the scene
        private void UnSetAssetRef()
        {
            if (stub.FlowManagerGO == null || stub.RecipeUIGO == null ||
                stub.AudioManagerGO == null || stub.PlayerSwitchingManagerGO == null || stub.BootstrapManagerGO == null)
            {
                return;
            }

            Component component = stub.FlowManagerGO.GetComponent<LevelIntroFlowroutine>();
            LevelIntroFlowroutineData m_data = (LevelIntroFlowroutineData)component.GetType()
                .GetField("m_data", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(component);
            m_data.GoUIPrefab = null;
            m_data.ReadyUIPrefab = null;
            m_data.TutorialPopup.Prefab = null;

            component = stub.RecipeUIGO.GetComponent<RecipeFlowGUI>();
            component.GetType()
                .GetField("m_recipeWidgetPrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);

            component = stub.AudioManagerGO.GetComponent<CampaignAudioManager>();
            component.GetType()
                .GetField("m_inLevelMusic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);
            component.GetType()
                .GetField("m_inLevelAmbiences", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameLoopingAudioTag[0]);
            component.GetType()
                .GetField("m_summaryScreenMusic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);
            AudioDirectoryData[] m_audioDirectories = (AudioDirectoryData[])typeof(AudioManager)
                .GetField("m_audioDirectories", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(component);
            for (int i = 0; i < m_audioDirectories.Length; i++)
            {
                m_audioDirectories[i] = null;
            }

            component = stub.PlayerSwitchingManagerGO.GetComponent<PlayerSwitchingManager>();
            component.GetType()
                .GetField("m_transitionParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);
            component.GetType()
                .GetField("m_transitionStartParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);
            component.GetType()
                .GetField("m_transitionEndParticlePrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);

            component = stub.BootstrapManagerGO.GetComponent<KitchenBootstrapManager>();
            component.GetType()
                .GetField("m_playerOneChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(null, null));
            component.GetType()
                .GetField("m_playerTwoChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(null, null));
            component.GetType()
                .GetField("m_playerThreeChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(null, null));
            component.GetType()
                .GetField("m_playerFourChef", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, new GameSession.SelectedChefData(null, null));
            typeof(BootstrapManager)
                .GetField("m_gameMetaEnvironmentPrefab", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);
            component.GetType()
                .GetField("m_bootstrapConfig", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(component, null);

            component = stub.KillPlaneGO.GetComponent<RespawnCollider>();
            (component as RespawnCollider).m_onDeathEffect = null;
        }

        public static AssetBundle GetAssetBundle(string bundleName)
        {
            return Instance.bundleDict[bundleName];
        }

        public static GameObject LoadAsset(PseudoPrefabSO pseudoPrefabSO)
        {
            AssetBundle bundle = GetAssetBundle(pseudoPrefabSO.bundleName);
            return bundle.LoadAsset<GameObject>(pseudoPrefabSO.assetPath);
        }

        public static T LoadAsset<T>(PseudoPrefabSO pseudoPrefabSO) where T : UnityEngine.Object
        {
            AssetBundle bundle = GetAssetBundle(pseudoPrefabSO.bundleName);
            return bundle.LoadAsset<T>(pseudoPrefabSO.assetPath);
        }

        public static Sprite LoadSpriteSubAsset(PseudoPrefabSO pseudoPrefabSO)
        {
            AssetBundle bundle = GetAssetBundle(pseudoPrefabSO.bundleName);
            var sprite = bundle.LoadAssetWithSubAssets<Sprite>(pseudoPrefabSO.assetPath);
            return sprite.Length > 0 ? sprite[0] : null;
        }

        public static Mesh LoadMeshSubAsset(PseudoPrefabSO pseudoPrefabSO)
        {
            AssetBundle bundle = GetAssetBundle(pseudoPrefabSO.bundleName);
            var mesh = bundle.LoadAssetWithSubAssets<Mesh>(pseudoPrefabSO.assetPath);
            return mesh.Length > 0 ? mesh[0] : null;
        }

        private AssetBundle LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {
            if (bundleDict.ContainsKey(assetBundleName) && bundleDict[assetBundleName] != null)
            {
                //Debug.Log("Loaded Asset Bundle : " + assetBundleName);
                return bundleDict[assetBundleName];
            }
            //Debug.Log("Loading Asset Bundle " + ((!isLoadingAssetBundleManifest) ? ": " : "Manifest: ") + assetBundleName);
            if (!isLoadingAssetBundleManifest && assetBundleManifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest");
                return null;
            }
            LoadAssetBundleInternal(assetBundleName);
            if (!isLoadingAssetBundleManifest)
            {
                string[] allDependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
                for (int j = 0; j < allDependencies.Length; j++)
                {
                    if (!bundleDict.ContainsKey(allDependencies[j]) || bundleDict[allDependencies[j]] == null)
                        LoadAssetBundleInternal(allDependencies[j]);
                }
            }
            return bundleDict[assetBundleName];
        }

        private void LoadAssetBundleInternal(string assetBundleName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Windows") + "/" + assetBundleName;

            //FileInfo fileInfo = new FileInfo(path);
            //long fileSizeInBytes = fileInfo.Length;
            //double fileSizeInMB = fileSizeInBytes / 1024.0 / 1024.0;
            //Debug.Log(string.Format("Loading bundle: {0}, size (MB): {1:F1}", assetBundleName, fileSizeInMB));

            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                Debug.LogError(string.Format("{0} is not a valid asset bundle.", assetBundleName));
            }
            else
            {
                bundleDict.SafeAdd(assetBundleName, assetBundle);
            }
        }

        private void UnloadAssetBundle(string assetBundleName)
        {
            if (bundleDict.ContainsKey(assetBundleName))
            {
                if (bundleDict[assetBundleName] != null)
                {
                    bundleDict[assetBundleName].Unload(true);
                    //Debug.Log(assetBundleName + " has been unloaded successfully.");
                }
                else
                {
                    //Debug.Log(assetBundleName + " to be unloaded is null.");
                }
                bundleDict.Remove(assetBundleName);
            }
            else
            {
                Debug.Log(assetBundleName + " to be unloaded is not loaded.");
            }
        }
    }
}