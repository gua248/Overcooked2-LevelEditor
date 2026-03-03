using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
    [ExecuteInEditMode]
    public class PseudoParticleSystem : MonoBehaviour
    {
        private PseudoParticleSystemStub stub;
        private ParticleSystem ps;
        private ParticleSystemRenderer psr;

        void Awake()
        {
            stub = GetComponent<PseudoParticleSystemStub>();
            ps = GetComponent<ParticleSystem>();
            psr = GetComponent<ParticleSystemRenderer>();

            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
            {
                Setup();
            }
        }

        public void Setup() 
        {
            Clear();
            psr.renderMode = ParticleSystemRenderMode.Mesh;

            if (stub.meshSO != null)
            {
                Mesh mesh = PseudoPrefabManager.LoadMeshSubAsset(stub.meshSO);
                psr.SetMeshes(new Mesh[] { mesh });
            }

            if (stub.materialSO != null)
            {
                Material material = PseudoPrefabManager.LoadAsset<Material>(stub.materialSO);
                psr.sharedMaterial = material;
            }
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            stub = GetComponent<PseudoParticleSystemStub>();
            ps = GetComponent<ParticleSystem>();
            psr = GetComponent<ParticleSystemRenderer>();

            psr.renderMode = ParticleSystemRenderMode.None;
            if (stub.materialSO != null)
            {
                psr.sharedMaterial = null;
            }

            if (stub.meshSO != null)
            {
                psr.mesh = null;
            }
        }
    }
}