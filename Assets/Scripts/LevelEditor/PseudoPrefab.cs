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

        public void ResetChild()
        {
            ClearChild();

            GameObject prefab = PseudoPrefabManager.LoadAsset(stub.pseudoPrefabSO);
            childGameObject = Instantiate(prefab, transform.position, transform.rotation, transform);
            childGameObject.name = stub.pseudoPrefabSO.prefabName;

            EditorGridSnap editorGridSnap = childGameObject.GetComponent<EditorGridSnap>();
            if (editorGridSnap != null && !Application.isPlaying &&
                childGameObject.GetComponent<PlateStation>() == null &&
                childGameObject.GetComponentInChildren<WashingStation>() == null &&
                childGameObject.GetComponent<TriggerZone>() == null)
            {
                editorGridSnap.enabled = true;
                editorGridSnap.GetType()
                    .GetField("m_constrainY", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(editorGridSnap, true);
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
            if (editorGridSnap != null && !Application.isPlaying && (
                childGameObject.GetComponent<Teleportal>() != null || 
                childGameObject.GetComponent<Plate>() != null ||
                childGameObject.GetComponent<FireExtinguishSpray>() != null
                ))
            {
                editorGridSnap.enabled = false;
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
                        Renderer renderer = childGameObject.GetComponent<Renderer>();
                        if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                        {
                            Material material = new Material(renderer.sharedMaterial);
                            material.SetColor("_Colour", new Color32(204, 204, 204, 255));
                            PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                        }
                        renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        break;
                    }

                case "noripple_m_dlc3_icecliff":
                    {
                        childGameObject.transform.Find("ripple").gameObject.SetActive(false);
                        break;
                    }

                case "p_dlc09_box_lid":
                case "p_dlc09_wallbit_01":
                    {
                        Renderer[] renderers = childGameObject.RequestComponentsRecursive<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                            {
                                Material material = new Material(renderer.sharedMaterial);
                                material.SetColor("_SnowColour", new Color32(179, 179, 179, 255));
                                PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                            }
                            renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
                        }
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
                        Renderer renderer = childGameObject.transform.Find(childGameObject.name.Replace("p_dlc09", "m_dlc5")).GetComponent<Renderer>();
                        if (!PseudoPrefabManager.Instance.editedMaterials.ContainsKey(renderer.sharedMaterial.name))
                        {
                            Material material = new Material(renderer.sharedMaterial);
                            material.SetColor("_SnowColour", new Color32(179, 179, 179, 255));
                            PseudoPrefabManager.Instance.editedMaterials.Add(renderer.sharedMaterial.name, material);
                        }
                        renderer.sharedMaterial = PseudoPrefabManager.Instance.editedMaterials[renderer.sharedMaterial.name];
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

                case "crate_raft_x2_01":
                    {
                        childGameObject.GetComponent<EditorGridSnap>().enabled = false;
                        break;
                    }

                default:
                    break;
            }
        }

        public void ClearChild()
        {
            if (stub == null)
                stub = GetComponent<PseudoPrefabStub>();
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