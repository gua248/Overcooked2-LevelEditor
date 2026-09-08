using LevelEditorStub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
    [ExecuteInEditMode]
    [SelectionBase]
    public class PseudoPrefab : MonoBehaviour
    {
        protected PseudoPrefabStub stub;

        public GameObject childGameObject;

        private void Awake()
        {
            stub = GetComponent<PseudoPrefabStub>();
        }

        private void Start()
        {
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
                ResetChild();
        }

        public virtual void ResetChild()
        {
            if (stub == null)
                stub = GetComponent<PseudoPrefabStub>();

            GameObject prefab = PseudoPrefabManager.LoadAsset(stub.pseudoPrefabSO);
            // ResetChild may be called again in LoadAsset(), instantiating a child object
            // thus ClearChild() here (not above), otherwise two child objects are instantiated
            ClearChild();
            childGameObject = Instantiate(prefab, transform.position, transform.rotation, transform);
            childGameObject.name = stub.pseudoPrefabSO.prefabName;

            EditorGridSnap editorGridSnap = childGameObject.GetComponent<EditorGridSnap>();
            if (editorGridSnap != null && !Application.isPlaying)
            {
                editorGridSnap.enabled = true;
                editorGridSnap.GetType()
                    .GetField("m_constrainY", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(editorGridSnap, false);
                if (childGameObject.GetComponent<Teleportal>() != null ||
                    childGameObject.GetComponent<PlateStation>() != null ||
                    childGameObject.GetComponentInChildren<WashingStation>() != null ||
                    childGameObject.GetComponent<TriggerZone>() != null ||
                    childGameObject.GetComponent<PhysicalAttachment>() != null ||
                    childGameObject.GetComponent<FurnaceCosmeticDecisions>() != null ||
                    childGameObject.GetComponent<AutoWorkstation>() != null)
                {
                    editorGridSnap.enabled = false;
                }
            }
            if (editorGridSnap != null &&
                gameObject.transform.FindParentRecursive("Animated Objects") != null)
            {
                editorGridSnap.enabled = false;
                var location = childGameObject.GetComponent<StaticGridLocation>();
                if (location != null)
                {
                    var dynamicGridLocation = childGameObject.AddComponent<DynamicGridLocation>();
                    DestroyImmediate(location);
                    dynamicGridLocation.enabled = false;
                    dynamicGridLocation.enabled = true;
                }
            }

            foreach (RendererInfo rendererInfo in childGameObject.RequestComponentsRecursive<RendererInfo>())
            {
                rendererInfo.lightmapIndex = -1;
                rendererInfo.lightmapScaleOffset = Vector4.zero;
            }

            SpecificPseudoPrefabTag specificPseudoPrefabTag = GetComponent<SpecificPseudoPrefabTag>();
            if (specificPseudoPrefabTag != null && !string.IsNullOrEmpty(specificPseudoPrefabTag.prefabTag))
            {
                HandleSpecificPrefabs(specificPseudoPrefabTag.prefabTag);
            }

            Setup();
        }

        private void HandleSpecificPrefabs(string tag)
        {
            switch (tag)
            {
                case "raft_water":
                    {
                        childGameObject.transform.Find("Reflection Plane").gameObject.SetActive(false);
                        childGameObject.transform.Find("sky").gameObject.SetActive(false);
                        break;
                    }

                case "PFX_background_wizardshool_01":
                    {
                        childGameObject.transform.Find("cloudgroup1").gameObject.SetActive(false);
                        childGameObject.transform.Find("cloudgroup2").gameObject.SetActive(false);
                        childGameObject.transform.Find("cloudgroup3").gameObject.SetActive(false);
                        childGameObject.transform.Find("cloudgroup4").gameObject.SetActive(false);
                        childGameObject.transform.Find("cloudgroup5").gameObject.SetActive(false);
                        childGameObject.transform.Find("sparkles (1)/sparkles (2)").gameObject.SetActive(false);
                        childGameObject.transform.Find("sparkles (1)/sparkles (3)").gameObject.SetActive(false);
                        childGameObject.transform.Find("sparkles (1)/sparkles (4)").gameObject.SetActive(false);
                        childGameObject.transform.Find("sparkles (1)/sparkles (5)").gameObject.SetActive(false);
                        childGameObject.transform.Find("background").gameObject.SetActive(false);
                        childGameObject.transform.Find("Planes_dummies").gameObject.SetActive(false);

                        var color = new ParticleSystem.MinMaxGradient(new Color32(104, 0, 255, 255), new Color32(0, 255, 202, 255));
                        var gradient = new Gradient();
                        gradient.SetKeys(new GradientColorKey[]
                        {
                        new GradientColorKey{color=Color.white, time=0f},
                        new GradientColorKey{color=Color.white, time=1f},
                        }, new GradientAlphaKey[]
                        {
                        new GradientAlphaKey{alpha=0f, time=0f},
                        new GradientAlphaKey{alpha=74/255f, time=.308f},
                        new GradientAlphaKey{alpha=107/255f, time=.641f},
                        new GradientAlphaKey{alpha=0f, time=1f},
                        });
                        var colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient);
                        Action<ParticleSystem> SetPFX = particleSystem =>
                        {
                            ParticleSystem.MainModule main;
                            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule;
                            Vector3 directionToCamera;
                            Quaternion rotationToCamera;
                            Vector3 eulerAngles;
                            main = particleSystem.main;
                            main.startColor = color;
                            colorOverLifetimeModule = particleSystem.colorOverLifetime;
                            colorOverLifetimeModule.color = colorOverLifetime;
                            directionToCamera = Camera.main.transform.position - particleSystem.transform.position;
                            directionToCamera = -particleSystem.transform.InverseTransformDirection(directionToCamera);
                            rotationToCamera = Quaternion.LookRotation(directionToCamera);
                            eulerAngles = rotationToCamera.eulerAngles;
                            main.startRotationX = new ParticleSystem.MinMaxCurve(eulerAngles.x * Mathf.Deg2Rad, eulerAngles.x * Mathf.Deg2Rad);
                            main.startRotationY = new ParticleSystem.MinMaxCurve(eulerAngles.y * Mathf.Deg2Rad, eulerAngles.y * Mathf.Deg2Rad);
                            main.startRotationZ = new ParticleSystem.MinMaxCurve(180 * Mathf.Deg2Rad, -180 * Mathf.Deg2Rad) { mode = ParticleSystemCurveMode.TwoConstants };
                            particleSystem.Stop();
                            particleSystem.Clear();
                            particleSystem.Play();
                        };
                        SetPFX(childGameObject.transform.Find("cloudgroup6").GetComponent<ParticleSystem>());
                        SetPFX(childGameObject.transform.Find("cloudgroup7").GetComponent<ParticleSystem>());
                        break;
                    }

                case "wizard_shelf_01":
                case "wizard_shelf_02":
                case "wizard_shelf_03":
                case "wizard_shelf_04":
                    {
                        Light[] lights = childGameObject.RequestComponentsRecursive<Light>();
                        foreach (Light light in lights)
                        {
#if UNITY_EDITOR
                            light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                            light.intensity *= 0.4f;
                        }
                        break;
                    }

                case "wizard_sconcecandle_01":
                    {
                        Light light = childGameObject.RequestComponentRecursive<Light>();
                        if (light != null)
                        {
#if UNITY_EDITOR
                            light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                            light.intensity *= 0.3f;
                            light.range *= 0.5f;
                        }
                        break;
                    }

                case "throne_torch":
                    {
                        Light light = childGameObject.RequestComponentRecursive<Light>();
                        if (light != null)
                        {
#if UNITY_EDITOR
                            light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                            light.color = new Color32(255, 153, 9, 255);
                            light.range = 5f;
                        }
                        break;
                    }

                case "m_sp_cliff":
                    {
                        Renderer renderer = childGameObject.RequestComponentRecursive<Renderer>();
                        Material[] materials = renderer.sharedMaterials;
                        renderer.sharedMaterials = new Material[] { materials[0], materials[1] };
                        renderer.receiveShadows = true;
                        break;
                    }

                case "sp_cliff":
                    {
                        Renderer renderer = childGameObject.RequestComponentRecursive<Renderer>();
                        renderer.receiveShadows = true;
                        break;
                    }

                case "sp_rock_01":
                    {
                        childGameObject.transform.Find("Point light").gameObject.SetActive(false);
                        childGameObject.transform.Find("Point light (1)").gameObject.SetActive(false);
                        break;
                    }

                case "Alien_Tentacle_01":
                    {
                        childGameObject.AddComponent<AnimatorAudioComponent>();
                        break;
                    }

                case "restaurant_lantern":
                case "restaurant_light_01":
                    {
                        Light light = childGameObject.RequestComponentRecursive<Light>();
                        if (light != null)
                        {
#if UNITY_EDITOR
                            light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                            light.range = 3f;
                            light.intensity = 1f;
                        }
                        break;
                    }

                case "decoration_wall_light 1":
                    {
                        Light[] lights = childGameObject.RequestComponentsRecursive<Light>();
                        foreach (Light light in lights)
                        {
#if UNITY_EDITOR
                            light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                        }
                        break;
                    }

                case "m_kitchen_firepit_02":
                    {
                        childGameObject.transform.Find("PFX_Fire_Hazzard (1)").gameObject.SetActive(false);
                        break;
                    }

                case "fire_hazard":
                    {
                        childGameObject.transform.Find("heathaze").gameObject.SetActive(false);
                        childGameObject.transform.Find("glow (1)").gameObject.SetActive(false);

                        ParticleSystem ps = childGameObject.transform.Find("PFX_FireStatic").GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule main = ps.main;
                        main.startLifetime = 2f;
                        main.startSpeed = 0f;
                        main.startSize = 0.8f;
                        main.gravityModifier = -0.35f;
                        main.maxParticles = 50;
                        ParticleSystem.EmissionModule emission = ps.emission;
                        emission.rateOverTime = 20;
                        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
                        var colorKeys = colorOverLifetime.color.gradient.colorKeys;
                        var gradient = new Gradient();
                        gradient.SetKeys(colorKeys, new GradientAlphaKey[]
                        {
                            new GradientAlphaKey{alpha=180/255f, time=0f},
                            new GradientAlphaKey{alpha=0f, time=0.479f},
                        });
                        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
                        break;
                    }

                case "exterior_car":
                    {
                        childGameObject.AddComponent<AnimatorAudioComponent>();
                        break;
                    }

                case "p_dlc5_camp_fire_02":
                    {
                        childGameObject.transform.Find("pfx/Light").gameObject.SetActive(false);
                        ParticleSystem ps = childGameObject.transform.Find("pfx/glow (1)").GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule main = ps.main;
                        main.startSize = 1.6f;
                        break;
                    }

                case "p_dlc5_camp_fire_02_nopfx":
                    {
                        childGameObject.transform.Find("pfx").gameObject.SetActive(false);
                        break;
                    }

                case "snow":
                    {
                        HandleSpecificPrefabs_Snow();
                        break;
                    }

                case "noripple_m_dlc3_icecliff":
                    {
                        childGameObject.transform.Find("ripple").gameObject.SetActive(false);
                        break;
                    }

                case "p_dlc09_box_lid":
                case "p_dlc09_wallbit_01":
                case "p_dlc09_snow":
                    {
                        HandleSpecificPrefabs_Snow_DLC09();
                        break;
                    }

                case "NPC_Penguin":
                    {
                        Material material = gameObject.GetComponent<Renderer>().sharedMaterial;
                        childGameObject.transform.Find("Penguin1:RoadKillOut").GetComponent<Renderer>().sharedMaterial = material;
                        break;
                    }

                case "DogSled":
                case "DogSled_Luggage":
                    {
                        Material material = gameObject.GetComponent<Renderer>().sharedMaterial;
                        childGameObject.GetComponent<Renderer>().sharedMaterial = material;
                        break;
                    }

                case "p_dlc09_tent":
                    {
                        childGameObject.transform.Find("glow").gameObject.SetActive(false);
                        childGameObject.transform.Find("Point light").gameObject.SetActive(false);
                        HandleSpecificPrefabs_Snow_DLC09();
                        break;
                    }

                case "Space_Door_Airlock_Open":
                case "Space_Door_Airlock_Close":
                case "Space_Door_Airlock_Bool_Open":
                case "Space_Door_Airlock_Bool_Close":
                    {
                        childGameObject.AddComponent<AnimatorAudioComponent>();
                        Animator animator = childGameObject.GetComponent<Animator>();
                        foreach (var trigger in gameObject.GetComponents<TriggerOnAnimator>())
                            trigger.m_targetAnimator = animator;
                        foreach (var trigger in gameObject.GetComponents<TriggerAnimatorSetVariable>())
                            trigger.m_targetAnimator = animator;
                        if (tag == "Space_Door_Airlock_Open")
                            animator.SetTrigger("Open");
                        if (tag == "Space_Door_Airlock_Bool_Open")
                            animator.SetBool("IsOpen", true);
                        break;
                    }

                case "p_dlc07_keep_flagstone":
                    {
                        Renderer[] renderers = childGameObject.RequestComponentsRecursive<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                            {
                                Material material = new Material(renderer.sharedMaterial);
                                material.SetColor("_ColourOverlay", new Color32(40, 40, 40, 255));
                                PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                            }
                            renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        }
                        break;
                    }

                case "ChoppingCounter":
                    {
                        if (stub.pseudoPrefabSO.prefabName == "countertop_01_chopping_board_gold")
                        {
                            foreach (Transform child in childGameObject.transform)
                            {
                                if (child.gameObject.name == "PFX_MagicCloud_Levitate")
                                    child.gameObject.SetActive(false);
                            }
                        }
                        break;
                    }

                case "ServingStation":
                    {
                        if (stub.pseudoPrefabSO.prefabName == "workstation_plate_station_slim_01_no_block")
                        {
                            childGameObject.transform.Find("Block_Back").gameObject.SetActive(false);
                        }
                        break;
                    }

                case "Beach_AirParrot":
                    {
                        foreach (Transform t in childGameObject.GetComponentsInChildren<Transform>())
                        {
                            if (t.name.StartsWith("Bird_takeoff_flight_and_landing:"))
                                t.name = t.name.Substring(32);
                        }
                        childGameObject.transform.Find("m_Beach_Parrot_02").GetComponent<Animator>().Rebind();
                        break;
                    }

                case "crate_raft":
                    {
                        if (childGameObject.GetComponent<EditorGridSnap>() != null)
                            childGameObject.GetComponent<EditorGridSnap>().enabled = false;
                        Material mat = PseudoPrefabManager.LoadAsset<Material>(GetComponent<PseudoPrefabSOArray>().pseudoPrefabSOs[0]);
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        Material mat1 = renderer.sharedMaterials[1];
                        renderer.sharedMaterials = new Material[] { mat, mat1 };
                        break;
                    }

                case "RopeSwing_Group03":
                    {
                        Transform parent = childGameObject.transform.Find("AnimationPivot/Child_PivotOffset/Child_Animation/p_dlc5_hanging_masonjar (3)/m_dlc5_hanging_masonjar_01");
                        foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>())
                            renderer.enabled = true;
                        break;
                    }

                case "p_dlc5_throne_01":
                    {
                        PseudoPrefabSO matSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        matSO.prefabName = "mat_dlc5_throne_01";
                        matSO.bundleName = "bundle248";
                        matSO.assetPath = "Assets\\downloadablecontent\\dlc05\\dlc_assets\\models\\dressing assets\\materials\\mat_dlc5_ivy_01.mat".Replace("\\", "/");
                        Material mat1 = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        matSO.prefabName = "mat_dlc5_foliage_01";
                        matSO.bundleName = "bundle248";
                        matSO.assetPath = "Assets\\downloadablecontent\\dlc05\\dlc_assets\\models\\dressing assets\\materials\\mat_dlc5_foliage_01.mat".Replace("\\", "/");
                        Material mat0 = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        childGameObject.transform.Find("m_dlc5_vines_A").GetComponent<Renderer>().sharedMaterials = new Material[2] { mat0, mat1 };
                        childGameObject.transform.Find("m_dlc5_vines_B").GetComponent<Renderer>().sharedMaterials = new Material[2] { mat0, mat1 };
                        DestroyImmediate(matSO);
                        break;
                    }

                case "Buoy_01":
                    {
                        PseudoPrefabSO[] matSOs = GetComponent<PseudoPrefabSOArray>().pseudoPrefabSOs;
                        Material mat0 = PseudoPrefabManager.LoadAsset<Material>(matSOs[0]);
                        Material mat1 = PseudoPrefabManager.LoadAsset<Material>(matSOs[1]);
                        childGameObject.transform.Find("Buoy_01/Buoy_01").GetComponent<Renderer>().sharedMaterial = mat0;
                        childGameObject.transform.Find("Buoy_01/ripple_1 (11)").GetComponent<Renderer>().sharedMaterial = mat1;
                        break;
                    }

                case "Scooter_02":
                    {
                        PseudoPrefabSO[] matSOs = GetComponent<PseudoPrefabSOArray>().pseudoPrefabSOs;
                        Material mat = PseudoPrefabManager.LoadAsset<Material>(matSOs[0]);
                        foreach (Renderer renderer in childGameObject.GetComponentsInChildren<Renderer>())
                            if (renderer.gameObject.name.StartsWith("m_dlc2_scooter"))
                                renderer.sharedMaterial = mat;
                        break;
                    }

                case "Barbeque":
                    {
                        childGameObject.GetComponent<HeatedStationGUI>().m_Offset = new Vector3(0f, 1f, 0f);
                        break;
                    }

                case "dlc03_diners_01":
                    {
                        PseudoPrefabSO matSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        matSO.prefabName = "Plate";
                        matSO.bundleName = "bundle38";
                        matSO.assetPath = "Assets\\models\\props\\materials\\Plate.mat".Replace("\\", "/");
                        Material mat1 = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        childGameObject.transform.Find("mince_pies_01/m_sk_plate_02").GetComponent<Renderer>().sharedMaterial = mat1;
                        DestroyImmediate(matSO);
                        break;
                    }

                case "iceberg":
                    {
                        Renderer[] renderers = childGameObject.RequestComponentsRecursive<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                            {
                                Material material = new Material(renderer.sharedMaterial);
                                material.SetFloat("_Power_01", 0.002f);
                                PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                            }
                            renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        }
                        break;
                    }

                case "dlc03_stall":
                    {
                        HandleSpecificPrefabs_DisableLights();
                        HandleSpecificPrefabs_Snow();
                        break;
                    }

                case "wind":
                    {
                        EditorGridSnap editorGridSnap = childGameObject.GetComponent<EditorGridSnap>();
                        FlowbasedComponentActivation flowbasedComponentActivation = childGameObject.GetComponent<FlowbasedComponentActivation>();
                        WindCosmeticDecisions windCosmeticDecisions = childGameObject.GetComponent<WindCosmeticDecisions>();
                        if (editorGridSnap != null) 
                            editorGridSnap.enabled = false;
                        if (flowbasedComponentActivation != null)
                        {
                            flowbasedComponentActivation.GetType()
                                .GetField("m_activeInRound", BindingFlags.Instance | BindingFlags.NonPublic)
                                .SetValue(flowbasedComponentActivation, false);
                            flowbasedComponentActivation.GetType()
                                .GetField("m_activeOutOfRound", BindingFlags.Instance | BindingFlags.NonPublic)
                                .SetValue(flowbasedComponentActivation, false);
                        }
                        if (windCosmeticDecisions != null)
                            windCosmeticDecisions.enabled = false;
                        break;
                    }

                case "m_streetlamp_01":
                    {
                        childGameObject.transform.Find("Point light").gameObject.SetActive(false);
                        HandleSpecificPrefabs_Snow();
                        break;
                    }

                case "utensil_big_ol_spoon":
                    {
                        childGameObject.GetComponent<IngredientContainer>().m_capacity = 32;
                        break;
                    }

                case "p_dlc4_stonebase":
                    {
                        PseudoPrefabSO matSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        matSO.prefabName = "mat_dlc4_mud_03";
                        matSO.bundleName = "bundle225";
                        matSO.assetPath = "Assets\\downloadablecontent\\dlc04\\dlc_assets\\models\\dressingassets\\materials\\mat_dlc4_mud_03.mat".Replace("\\", "/");
                        Material mat1 = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        childGameObject.GetComponent<Renderer>().sharedMaterial = mat1;
                        DestroyImmediate(matSO);
                        break;
                    }

                case "p_dlc4_floortile_01":
                case "p_dlc4_mud_01":
                case "p_dlc4_grass_01":
                    {
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                        {
                            Material material = new Material(renderer.sharedMaterial);
                            material.SetTextureScale("_Albedo", Vector2.one);
                            material.SetTextureScale("_Normal", Vector2.one);
                            material.SetTextureScale("_RMEA", Vector2.one);
                            PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                        }
                        renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        break;
                    }

                case "p_dlc4_water_01":
                    {
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                        {
                            Material material = new Material(renderer.sharedMaterial);
                            material.SetTextureScale("_Diffuse_Map", 0.2f * Vector2.one);
                            material.SetTextureScale("_NormalMap1", Vector2.one);
                            material.SetTextureScale("_NormalMap2", Vector2.one);
                            material.SetTextureOffset("_NormalMap2", Vector2.zero);
                            PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                        }
                        renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        break;
                    }

                case "p_dlc4_crazypaving_01":
                    {
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        renderer.sharedMaterials = new Material[] { renderer.sharedMaterials[1] };
                        if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                        {
                            Material material = new Material(renderer.sharedMaterial);
                            material.SetTextureScale("_Albedo", Vector2.one);
                            material.SetTextureScale("_Normal", Vector2.one);
                            material.SetTextureScale("_RMEA", Vector2.one);
                            PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                        }
                        renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        break;
                    }

                case "utensil_coalbucket_01":
                    {
                        ItemContainer itemContainer = childGameObject.GetComponent<ItemContainer>();
                        itemContainer.m_approvedContentsList = (OrderToPrefabLookup)typeof(OverlapModelsMealDecisions)
                            .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.NonPublic)
                            .GetValue(itemContainer.m_cosmeticsPrefab.GetComponent<CoalBucketCosmeticDecisions>());
                        break;
                    }

                case "p_dlc07_sconce_01":
                case "p_dlc07_courtyard_candelabra_01":
                case "p_dlc08_wagon":
                case "p_dlc08_sconce_01":
                case "DisableLights":
                    {
                        HandleSpecificPrefabs_DisableLights();
                        break;
                    }

                case "p_dlc07_traffic_light_01":
                case "m_dlc07_drawbridge":
                    {
                        if (GetComponent<Animator>() != null)
                            GetComponent<Animator>().Rebind();
                        break;
                    }

                case "dlc08_workstation_mixer":
                    {
                        if (childGameObject.GetComponent<EditorGridSnap>() != null)
                            childGameObject.GetComponent<EditorGridSnap>().enabled = false;
                        childGameObject.transform.Find("m_cakemixer_Body_01/Mesh/m_dlc7_city_countertop_01").gameObject.SetActive(false);
                        break;
                    }

                case "dlc08_condiment_dispenser":
                case "dlc08_drink_machine":
                case "dlc11_drink_dispenser":
                case "TriggerOnChild":
                    {
                        foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
                        {
                            componet.m_targetObject = childGameObject;
                        }
                        break;
                    }

                case "dlc11_condiment_dispenser":
                    {
                        foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
                        {
                            componet.m_targetObject = childGameObject;
                        }
                        PseudoPrefabSOArray pseudoPrefabSOArray = GetComponent<PseudoPrefabSOArray>();
                        if (pseudoPrefabSOArray != null && !pseudoPrefabSOArray.pseudoPrefabSOs.IsEmpty())
                        {
                            IngredientOrderNode[] nodes = pseudoPrefabSOArray.pseudoPrefabSOs.Select(x => RecipeHelper.GetIngredientOrderNode(x)).ToArray();
                            childGameObject.GetComponent<PlacementItemSwitcher>().m_ingredients = nodes;
                            childGameObject.GetComponent<IngredientPropertiesComponent>().SetIngredientOrderNode(nodes[0]);
                        }
                        break;
                    }

                case "p_dlc08_string_lights_01":
                    {
                        PseudoPrefabSO matSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        matSO.prefabName = "mat_dlc08_dressingassets_03";
                        matSO.bundleName = "bundle355";
                        matSO.assetPath = "Assets\\downloadablecontent\\dlc08\\dlc_assets\\models\\dressing assets\\materials\\mat_dlc08_dressingassets_03.mat".Replace("\\", "/");
                        Material mat1 = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        Material mat0 = renderer.sharedMaterials[0];
                        renderer.sharedMaterials = new Material[] { mat0, mat1, mat1 };
                        DestroyImmediate(matSO);
                        break;
                    }

                case "p_dlc09_battlements_wallsection":
                    {
                        Material mat = null;
                        foreach (var renderer in childGameObject.RequestComponentsRecursive<Renderer>())
                        {
                            if (renderer.sharedMaterial.name == "mat_dlc09_battlements_bricks_01")
                            {
                                mat = renderer.sharedMaterial;
                                break;
                            }
                        }
                        if (mat != null)
                        {
                            foreach (var renderer in childGameObject.RequestComponentsRecursive<Renderer>())
                                if (renderer.sharedMaterial.name == "mat_dlc09_battlements_bricks_02")
                                    renderer.sharedMaterial = mat;
                        }
                        HandleSpecificPrefabs_Snow_DLC09();
                        break;
                    }

                case "p_dlc09_hanging_masonjar":
                    {
                        HandleSpecificPrefabs_Snow_DLC09();
                        childGameObject.transform.Find("Particle System").gameObject.SetActive(false);
                        childGameObject.transform.Find("glow (1)").gameObject.SetActive(false);
                        childGameObject.transform.Find("Point light").gameObject.SetActive(false);
                        break;
                    }

                case "p_dlc09_hanging_masonjar_03":
                case "p_dlc09_building_01":
                case "p_dlc09_camp_fire_02":
                case "p_dlc09_lantern_01":
                    {
                        HandleSpecificPrefabs_DisableLights();
                        HandleSpecificPrefabs_Snow_DLC09();
                        break;
                    }

                case "p_dlc09_sconce_01":
                case "p_dlc09_wagon":
                    {
                        HandleSpecificPrefabs_DisableLights();
                        HandleSpecificPrefabs_Snow_DLC09();
                        foreach (Renderer renderer in childGameObject.RequestComponentsRecursive<MeshRenderer>())
                            renderer.enabled = true;
                        break;
                    }

                case "p_dlc09_snow_2":
                    {
                        HandleSpecificPrefabs_DisableLights();
                        HandleSpecificPrefabs_Snow_DLC09_2();
                        break;
                    }

                case "CounterCornerFloat":
                    {
                        DestroyImmediate(childGameObject.GetComponent<AnticipateInteractionHighlight>());
                        DestroyImmediate(childGameObject.GetComponent<TabletopConveyenceWindReceiver>());
                        DestroyImmediate(childGameObject.GetComponent<TabletopConveyenceReceiver>());
                        DestroyImmediate(childGameObject.GetComponent<AttachStation>());
                        break;
                    }

                case "p_seaweedfloat_01":
                    {
                        PseudoPrefabSO matSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        matSO.prefabName = "mat_seaweed_02";
                        matSO.bundleName = "bundle448";
                        matSO.assetPath = "Assets\\downloadablecontent\\dlc13\\assets\\models\\dressing\\materials\\mat_seaweed_02.mat".Replace("\\", "/");
                        Material mat = PseudoPrefabManager.LoadAsset<Material>(matSO);
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        renderer.sharedMaterial = mat;
                        DestroyImmediate(matSO);
                        break;
                    }

                case "dlc13_lotuspressureswitch_large":
                case "dlc13_lotuspressureswitch_small":
                    {
                        if (childGameObject.GetComponent<EditorGridSnap>() != null)
                            childGameObject.GetComponent<EditorGridSnap>().enabled = false;
                        break;
                    }

                case "dlc13_lotuspressureswitch_small_2":
                    {
                        if (childGameObject.GetComponent<EditorGridSnap>() != null)
                            childGameObject.GetComponent<EditorGridSnap>().enabled = false;
                        childGameObject.transform.Find("DLC13_LotusPressureSwitch (1)/PlatformCollisions/Art/m_lilypad_01 (4) Group/m_lilypad_flower_02 (1)").gameObject.SetActive(false);
                        break;
                    }

                case "cobble":
                    {
                        Renderer[] renderers = childGameObject.RequestComponentsRecursive<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                            {
                                Material material = new Material(renderer.sharedMaterial);
                                material.SetTexture("_Detail", null);
                                PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                            }
                            renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        private void HandleSpecificPrefabs_DisableLights()
        {
            foreach (Light light in childGameObject.RequestComponentsRecursive<Light>())
            {
                light.gameObject.SetActive(false);
            }
        }

        private void HandleSpecificPrefabs_Snow()
        {
            Renderer[] renderers = childGameObject.RequestComponentsRecursive<MeshRenderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                int snowIndex = -1;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name == "mat_dlc3_snow_01")
                    {
                        snowIndex = i;
                        break;
                    }
                }
                if (snowIndex == -1) continue;
                if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(materials[snowIndex].name))
                {
                    Material material = new Material(materials[snowIndex]);
                    material.SetColor("_Colour", new Color32(204, 204, 204, 255));
                    PseudoPrefabManager.Instance.editedMaterials.Add(materials[snowIndex].name, material);
                }
                Material snowMat = PseudoPrefabManager.Instance.editedMaterials[materials[snowIndex].name];
                renderer.sharedMaterials = materials.Select((x, i) => i == snowIndex ? snowMat : x).ToArray();
            }
        }

        private void HandleSpecificPrefabs_Snow_DLC09()
        {
            Renderer[] renderers = childGameObject.RequestComponentsRecursive<MeshRenderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (!material.HasProperty("_SnowColour")) continue;
                    if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(material.name))
                    {
                        Material newMaterial = new Material(material);
                        newMaterial.SetColor("_SnowColour", new Color32(179, 179, 179, 255));
                        PseudoPrefabManager.Instance.editedMaterials.Add(material.name, newMaterial);
                    }
                    materials[i] = PseudoPrefabManager.Instance.editedMaterials[material.name];
                }
                renderer.sharedMaterials = materials;
            }
        }

        private void HandleSpecificPrefabs_Snow_DLC09_2()
        {
            Renderer[] renderers = childGameObject.RequestComponentsRecursive<MeshRenderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (!material.name.StartsWith("mat_dlc9_snow_") || !material.HasProperty("_Colour")) continue;
                    if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(material.name))
                    {
                        Material newMaterial = new Material(material);
                        newMaterial.SetColor("_Colour", new Color32(169, 169, 169, 255));
                        PseudoPrefabManager.Instance.editedMaterials.Add(material.name, newMaterial);
                    }
                    materials[i] = PseudoPrefabManager.Instance.editedMaterials[material.name];
                }
                renderer.sharedMaterials = materials;
            }
        }

        public void ClearChild()
        {
            Cleanup();

            var children = transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);
            if (childGameObject != null)
                DestroyImmediate(childGameObject);
            childGameObject = null;

            SpecificPseudoPrefabTag specificPseudoPrefabTag = GetComponent<SpecificPseudoPrefabTag>();
            if (specificPseudoPrefabTag != null && !string.IsNullOrEmpty(specificPseudoPrefabTag.prefabTag))
            {
                HandleSpecificPrefabsClear(specificPseudoPrefabTag.prefabTag);
            }
        }

        private void HandleSpecificPrefabsClear(string tag)
        {
            switch (tag)
            {
                case "Space_Door_Airlock_Open":
                case "Space_Door_Airlock_Close":
                case "Space_Door_Airlock_Bool_Open":
                case "Space_Door_Airlock_Bool_Close":
                    {
                        foreach (var trigger in gameObject.GetComponents<TriggerOnAnimator>())
                        {
                            trigger.m_targetAnimator = null;
                        }
                        foreach (var trigger in gameObject.GetComponents<TriggerAnimatorSetVariable>())
                        {
                            trigger.m_targetAnimator = null;
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        public virtual void Setup()
        {
        }

        public virtual void LateSetup()
        {
        }

        public virtual void SetupAfterStartSynchronising()
        {
        }

        public virtual void Cleanup()
        {
        }

        private void Update()
        {
            if (childGameObject != null && !Application.isPlaying)
            {
                EditorGridSnap editorGridSnap = childGameObject.GetComponent<EditorGridSnap>();
                if (editorGridSnap != null && editorGridSnap.enabled)
                {
                    transform.position = childGameObject.transform.position;
                    childGameObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}