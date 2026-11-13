using GameModes.Horde;
using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class PseudoPrefabLockable : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabLockableStub lockableStub = (PseudoPrefabLockableStub)stub;
            childGameObject.GetComponent<HordeLockable>().m_unlockCost = lockableStub.unlockCost;
            foreach (Animator animator in childGameObject.RequestComponentsRecursive<Animator>())
            {
                animator.gameObject.AddComponent<AnimatorAudioComponent>();
            }
        }
    }
}