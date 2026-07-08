using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabSwitch : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabSwitchStub switchStub = (PseudoPrefabSwitchStub)stub;

            childGameObject.GetComponent<TriggerDisableScript>().m_startEnabled = switchStub.startEnabled;

            var cos = childGameObject.GetComponent<SwitchCosmeticDecisions>();
            cos.m_activeMaterial = PseudoPrefabManager.LoadAsset<Material>(switchStub.activeMaterial);
            cos.m_inactiveMaterial = PseudoPrefabManager.LoadAsset<Material>(switchStub.inactiveMaterial);
            cos.m_buttonBit.sharedMaterial = cos.m_activeMaterial;

            foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
            {
                componet.m_targetObject = childGameObject;
            }

            var triggerOnAnimator = childGameObject.GetComponent<TriggerOnAnimator>();
            triggerOnAnimator.m_triggerToFire = switchStub.triggerOnAnimator;
            triggerOnAnimator.m_targetAnimator = switchStub.animatorToTrigger;

            if (switchStub.objectToTrigger != null & switchStub.objectToTrigger.Length > 0)
            {
                var triggerOnObject = childGameObject.AddComponent<TriggerOnObject>();
                triggerOnObject.m_trigger = "Switch";
                triggerOnObject.m_triggerToFire = switchStub.triggerOnObject;
                if (switchStub.objectToTrigger.Length == 1)
                {
                    triggerOnObject.m_targetObject = switchStub.objectToTrigger[0];
                    triggerOnObject.m_targetObjects = new GameObject[0];
                }
                else
                {
                    triggerOnObject.m_targetObject = null;
                    triggerOnObject.m_targetObjects = switchStub.objectToTrigger;
                }
            }
        }

        public override void Cleanup()
        {
            foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
            {
                componet.m_targetObject = null;
            }
        }
    }
}