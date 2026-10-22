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
        private bool isSetup = false;

        protected virtual void Awake()
        {
            stub = GetComponent<SetupCustomPrefabStub>();
            // at runtime
            // custom prefabs can be in the scene or instantiated
            // for custom prefabs in the scene, Setup & LateSetup are called in PseudoPrefabManager
            // for custom prefabs instantiated at runtime, Setup is called here (such prefabs should not have LateSetup)
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Game)
            {
                if (!isSetup)
                {
                    Setup();
                }
            }
        }

        private void Start()
        {
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
                Setup();
        }

        public virtual void Setup()
        {
            stub = GetComponent<SetupCustomPrefabStub>();
            isSetup = true;
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