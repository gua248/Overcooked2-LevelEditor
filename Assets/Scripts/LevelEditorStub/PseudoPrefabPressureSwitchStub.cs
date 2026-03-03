using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabPressureSwitchStub : PseudoPrefabStub
    {
        [SerializeField] public PseudoPrefabSO occupiedMaterialSO;
        [SerializeField] public PseudoPrefabSO unoccupiedMaterialSO;
        [SerializeField] public string triggerOnAnimatorEnter;
        [SerializeField] public string triggerOnAnimatorExit;
        [SerializeField] public Animator animatorToTrigger;
        [SerializeField] public string triggerOnObjectEnter;
        [SerializeField] public string triggerOnObjectExit;
        [SerializeField] public GameObject[] objectToTrigger;
    }
}