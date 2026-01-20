using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabNPC : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabNPCStub NPCStub = (PseudoPrefabNPCStub)stub;
            RuntimeAnimatorController controller = PseudoPrefabManager.LoadAsset<RuntimeAnimatorController>(NPCStub.animatorControllerSO);
            Animator animator = childGameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = controller;
            }

            // HandleSpecificPrefabs
            if (stub.pseudoPrefabSO.prefabName == "NPC_Walk_Anticlockwise")
            {
                animator = childGameObject.transform.Find("Path").GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }
        }
    }
}