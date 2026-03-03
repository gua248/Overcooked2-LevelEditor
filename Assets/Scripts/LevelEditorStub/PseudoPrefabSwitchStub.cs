using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabSwitchStub : PseudoPrefabStub
    {
        [SerializeField] public bool startEnabled = true;
        [SerializeField] public PseudoPrefabSO activeMaterial;
        [SerializeField] public PseudoPrefabSO inactiveMaterial;
        [SerializeField] public string triggerOnAnimator;
        [SerializeField] public Animator animatorToTrigger;
        [SerializeField] public string triggerOnObject;
        [SerializeField] public GameObject[] objectToTrigger;
    }
}