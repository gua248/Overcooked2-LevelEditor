using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabPressureSwitch : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabPressureSwitchStub switchStub = (PseudoPrefabPressureSwitchStub)stub;
            PressureSwitchCosmeticDecisions cos = childGameObject.GetComponent<PressureSwitchCosmeticDecisions>();
            cos.m_occupiedMaterial = PseudoPrefabManager.LoadAsset<Material>(switchStub.occupiedMaterialSO);
            cos.m_unoccuppiedMaterial = PseudoPrefabManager.LoadAsset<Material>(switchStub.unoccupiedMaterialSO);
            cos.m_buttonBit.sharedMaterial = cos.m_unoccuppiedMaterial;

            var triggerOnObject = childGameObject.GetComponents<TriggerOnObject>();
            triggerOnObject[0].m_triggerToFire = switchStub.triggerOnObjectEnter;
            triggerOnObject[1].m_triggerToFire = switchStub.triggerOnObjectExit;
            if (switchStub.objectToTrigger != null & switchStub.objectToTrigger.Length > 0)
            {
                if (switchStub.objectToTrigger.Length == 1)
                {
                    triggerOnObject[0].m_targetObject = switchStub.objectToTrigger[0];
                    triggerOnObject[0].m_targetObjects = new GameObject[0];
                    triggerOnObject[1].m_targetObject = switchStub.objectToTrigger[0];
                    triggerOnObject[1].m_targetObjects = new GameObject[0];
                }
                else
                {
                    triggerOnObject[0].m_targetObject = null;
                    triggerOnObject[0].m_targetObjects = switchStub.objectToTrigger;
                    triggerOnObject[1].m_targetObject = null;
                    triggerOnObject[1].m_targetObjects = switchStub.objectToTrigger;
                }
            }

            if (switchStub.animatorToTrigger != null)
            {
                var triggerOnAnimatorEnter = childGameObject.AddComponent<TriggerOnAnimator>();
                triggerOnAnimatorEnter.m_targetAnimator = switchStub.animatorToTrigger;
                triggerOnAnimatorEnter.m_triggerToReceive = "Entered";
                triggerOnAnimatorEnter.m_triggerToFire = switchStub.triggerOnAnimatorEnter;
                var triggerOnAnimatorExit = childGameObject.AddComponent<TriggerOnAnimator>();
                triggerOnAnimatorExit.m_targetAnimator = switchStub.animatorToTrigger;
                triggerOnAnimatorExit.m_triggerToReceive = "Exited";
                triggerOnAnimatorExit.m_triggerToFire = switchStub.triggerOnAnimatorExit;
            }
        }
    }
}