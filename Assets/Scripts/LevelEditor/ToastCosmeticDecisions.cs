using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class ToastCosmeticDecisions : MealCosmeticDecisions
    {
        private ToastCosmeticDecisionsStub stub;

        private GameObject m_lid;
        private GameObject m_base;
        private GameObject m_empty;
        private bool containsBap;

        public OrderToPrefabLookup m_prefabLookup;
        public OrderToPrefabLookup m_sauceInPlatePrefabLookup;
        public OrderToPrefabLookup m_sauceOnToastPrefabLookup;
        public GameObject m_wrongSauceInPlatePrefab;
        public GameObject m_wrongSauceOnToastPrefab;
        public OrderDefinitionNode m_bapOrderDefinition;

        private void Awake()
        {
            stub = GetComponent<ToastCosmeticDecisionsStub>();
            m_lid = stub.m_lid;
            m_base = stub.m_base;
            m_empty = stub.m_empty;
        }
        
        protected override void Start()
        {
            if (m_prefabLookup != null)
            {
                m_prefabLookup.CacheAssembledOrderNodes();
            }
            if (m_sauceInPlatePrefabLookup != null)
            {
                m_sauceInPlatePrefabLookup.CacheAssembledOrderNodes();
            }
            if (m_sauceOnToastPrefabLookup != null)
            {
                m_sauceOnToastPrefabLookup.CacheAssembledOrderNodes();
            }
            base.Start();
            MeshFilter meshFilter = m_base.RequireComponentRecursive<MeshFilter>();
            Vector3 position = meshFilter.transform.TransformPoint(new Vector3(0f, meshFilter.sharedMesh.bounds.size.y, 0f));
            m_container.transform.localPosition = m_container.transform.parent.InverseTransformPoint(position);
            UpdateAppearance(m_iOrderDefinition.GetOrderComposition());
        }

        protected override IClientOrderDefinition FindOrderDefinition()
        {
            return gameObject.RequestInterfaceUpwardsRecursive<IClientOrderDefinition>();
        }

        protected override void UpdateAppearance(AssembledDefinitionNode _contents)
        {
            AssembledDefinitionNode assembledDefinitionNode = _contents.Simpilfy();
            CompositeAssembledNode compositeAssembledNode = assembledDefinitionNode as CompositeAssembledNode;
            if (compositeAssembledNode != null && compositeAssembledNode.m_composition.Length > 0)
            {
                bool active = ContainsBap(compositeAssembledNode);
                m_lid.SetActive(active);
                m_base.SetActive(active);
                m_empty.SetActive(false);
                LayoutContents(compositeAssembledNode);
            }
            else
            {
                m_lid.SetActive(false);
                m_base.SetActive(false);
                m_empty.SetActive(true);
                DestroyContents();
            }
        }

        private bool ContainsBap(CompositeAssembledNode _composite)
        {
            for (int i = 0; i < _composite.m_composition.Length; i++)
            {
                if (AssembledDefinitionNode.Matching(_composite.m_composition[i], m_bapOrderDefinition))
                {
                    containsBap = true;
                    return true;
                }
            }
            containsBap = false;
            return false;
        }

        private void DestroyContents()
        {
            for (int i = 0; i < m_container.transform.childCount; i++)
            {
                Destroy(m_container.transform.GetChild(i).gameObject);
            }
        }

        private void LayoutContents(CompositeAssembledNode _composite)
        {
            DestroyContents();
            Vector3 vector = Vector3.zero;
            for (int i = 0; i < _composite.m_composition.Length; i++)
            {
                GameObject prefab = GetIngredientPrefab(_composite.m_composition[i]);
                if (prefab != null)
                {
                    GameObject gameObject = prefab.InstantiateOnParent(m_container.transform, true);
                    MeshFilter meshFilter = gameObject.RequireComponentRecursive<MeshFilter>();
                    gameObject.transform.localPosition = vector;
                    float y = meshFilter.sharedMesh.bounds.size.y;
                    vector += Vector3.up * y * gameObject.transform.localScale.y;
                }
            }
            Vector3 position = m_container.transform.TransformPoint(vector);
            m_lid.transform.localPosition = m_lid.transform.parent.InverseTransformPoint(position);

            GameObject saucePrefab = GetSaucePrefab(_composite);
            if (saucePrefab != null)
            {
                GameObject sauce = saucePrefab.InstantiateOnParent(m_container.transform, true);
                MeshFilter lidMesh = m_lid.RequireComponentRecursive<MeshFilter>();
                float lidY = lidMesh.sharedMesh.bounds.size.y * m_lid.transform.lossyScale.y;
                if (containsBap)
                    vector += Vector3.up * lidY * m_lid.transform.localScale.y;
                sauce.transform.localPosition = vector;
            }
        }

        private GameObject GetIngredientPrefab(AssembledDefinitionNode _providedNode)
        {
            GameObject prefabForNode = m_prefabLookup.GetPrefabForNode(_providedNode);
            if (prefabForNode)
            {
                return prefabForNode;
            }
            return null;
        }

        private GameObject GetSaucePrefab(CompositeAssembledNode _composite)
        {
            int cnt = 0;
            OrderToPrefabLookup lookup = containsBap ? m_sauceOnToastPrefabLookup : m_sauceInPlatePrefabLookup;
            GameObject wrongSauce = containsBap ? m_wrongSauceOnToastPrefab : m_wrongSauceInPlatePrefab;
            GameObject prefabForNode = null;
            for (int i = 0; i < _composite.m_composition.Length; i++)
            {
                GameObject prefab = lookup.GetPrefabForNode(_composite.m_composition[i]);
                if (prefab)
                {
                    prefabForNode = prefab;
                    cnt++;
                }
            }
            if (cnt <= 1)
                return prefabForNode;

            return wrongSauce;
        }
    }
}