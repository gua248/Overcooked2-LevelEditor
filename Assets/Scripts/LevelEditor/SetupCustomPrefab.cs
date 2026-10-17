using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-99)]
    [SelectionBase]
    public abstract class SetupCustomPrefab : MonoBehaviour
    {
        protected SetupCustomPrefabStub stub;

        protected virtual void Awake()
        {
            stub = GetComponent<SetupCustomPrefabStub>();
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Game)
                Setup();
        }

        private void Start()
        {
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
                Setup();
        }

        public virtual void Setup()
        {
            stub = GetComponent<SetupCustomPrefabStub>();
        }

        public virtual void LateSetup()
        {
        }

        public virtual void Clear()
        {
            stub = GetComponent<SetupCustomPrefabStub>();
        }
    }
}