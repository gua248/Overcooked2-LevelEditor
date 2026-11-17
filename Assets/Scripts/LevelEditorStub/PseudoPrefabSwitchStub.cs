using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabSwitchStub : PseudoPrefabStub
    {
        [SerializeField] public bool startEnabled = true;
        [PseudoPrefabSOCategory("SwitchMaterial")]
        [SerializeField] public PseudoPrefabSO activeMaterial;
        [PseudoPrefabSOCategory("SwitchMaterial")]
        [SerializeField] public PseudoPrefabSO inactiveMaterial;
        [SerializeField] public string triggerOnAnimator;
        [SerializeField] public Animator animatorToTrigger;
        [SerializeField] public string triggerOnObject;
        [SerializeField] public GameObject[] objectToTrigger;
    }
}