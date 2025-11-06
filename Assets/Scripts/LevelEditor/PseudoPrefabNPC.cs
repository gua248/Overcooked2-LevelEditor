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
            childGameObject.GetComponent<Animator>().runtimeAnimatorController = controller;
        }
    }
}