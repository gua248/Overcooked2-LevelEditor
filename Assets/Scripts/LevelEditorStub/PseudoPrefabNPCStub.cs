using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabNPCStub : PseudoPrefabStub
    {
        [PseudoPrefabSOCategory("NPCAnimator")]
        [SerializeField] public PseudoPrefabSO animatorControllerSO;
    }
}