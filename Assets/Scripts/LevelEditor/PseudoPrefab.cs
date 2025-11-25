using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using LevelEditorStub;


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