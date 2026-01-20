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
    public class PseudoPrefab : MonoBehaviour
    {
        protected PseudoPrefabStub stub;

        public GameObject childGameObject;

        private void Awake()
        {
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
                ResetChild();
        }

        public void ResetChild()
        {
            stub = GetComponent<PseudoPrefabStub>();
            ClearChild();

            GameObject prefab = PseudoPrefabManager.LoadAsset(stub.pseudoPrefabSO);
            childGameObject = Instantiate(prefab, transform.position, transform.rotation, transform);
            childGameObject.name = stub.pseudoPrefabSO.prefabName;

            EditorGridSnap editorGridSnap = childGameObject.GetComponent<EditorGridSnap>();
            if (editorGridSnap != null && !Application.isPlaying &&
                childGameObject.GetComponent<PlateStation>() == null &&
                childGameObject.GetComponentInChildren<WashingStation>() == null)
            {
                editorGridSnap.enabled = true;
                editorGridSnap.GetType()
                    .GetField("m_constrainY", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(editorGridSnap, true);
            }
            if (editorGridSnap != null && !Application.isPlaying &&
                childGameObject.GetComponent<Teleportal>() != null)
            {
                editorGridSnap.enabled = false;
            }

            HandleSpecificPrefabs();

            Setup();
        }

        private void HandleSpecificPrefabs()
        {
            foreach (RendererInfo rendererInfo in childGameObject.RequestComponentsRecursive<RendererInfo>())
            {
                rendererInfo.lightmapIndex = -1;
                rendererInfo.lightmapScaleOffset = Vector4.zero;
            }

            if (false) { }
            else if (stub.pseudoPrefabSO.prefabName == "raft_water")
            {
                childGameObject.transform.Find("Reflection Plane").gameObject.SetActive(false);
                childGameObject.transform.Find("sky").gameObject.SetActive(false);
            }

            else if (stub.pseudoPrefabSO.prefabName == "PFX_background_wizardshool_01")
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
            }

            else if (stub.pseudoPrefabSO.prefabName.StartsWith("wizard_shelf"))
            {
                Light[] lights = childGameObject.RequestComponentsRecursive<Light>();
                foreach (Light light in lights)
                {
#if UNITY_EDITOR
                    light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                    light.intensity *= 0.4f;
                }
            }

            else if (stub.pseudoPrefabSO.prefabName == "wizard_sconcecandle_01")
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
            }

            else if (stub.pseudoPrefabSO.prefabName == "throne_torch")
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
            }

            else if (gameObject.name.StartsWith("m_sp_cliff"))
            {
                Renderer renderer = childGameObject.RequestComponentRecursive<Renderer>();
                Material[] materials = renderer.sharedMaterials;
                renderer.sharedMaterials = new Material[] { materials[0], materials[1] };
            }

            else if (stub.pseudoPrefabSO.prefabName == "sp_rock_01")
            {
                childGameObject.transform.Find("Point light").gameObject.SetActive(false);
                childGameObject.transform.Find("Point light (1)").gameObject.SetActive(false);
            }

            else if (stub.pseudoPrefabSO.prefabName == "Alien_Tentacle_01")
            {
                childGameObject.AddComponent<AnimatorAudioComponent>();
            }

            else if (stub.pseudoPrefabSO.prefabName == "restaurant_lantern" || stub.pseudoPrefabSO.prefabName == "restaurant_light_01")
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
            }

            else if (stub.pseudoPrefabSO.prefabName == "decoration_wall_light 1")
            {
                Light[] lights = childGameObject.RequestComponentsRecursive<Light>();
                foreach (Light light in lights)
                {
#if UNITY_EDITOR
                    light.lightmapBakeType = LightmapBakeType.Realtime;
#endif
                }
            }

            else if (gameObject.name.StartsWith("m_kitchen_firepit_02"))
            {
                childGameObject.transform.Find("PFX_Fire_Hazzard (1)").gameObject.SetActive(false);
            }

            else if (stub.pseudoPrefabSO.prefabName == "fire_hazard")
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
            }

            else if (stub.pseudoPrefabSO.prefabName.StartsWith("exterior_car_"))
            {
                childGameObject.AddComponent<AnimatorAudioComponent>();
            }
        }

        public void ClearChild()
        {
            var children = transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);
            if (childGameObject != null)
                DestroyImmediate(childGameObject);
            childGameObject = null;
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

        private void Update()
        {
            if (childGameObject != null && !Application.isPlaying)
            {
                transform.position = childGameObject.transform.position;
                childGameObject.transform.localPosition = Vector3.zero;
            }
        }
    }
}