using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    public class SetupCannonStub : SetupCustomPrefabStub
    {
        [SerializeField] public PseudoPrefabSO modelSO;
        [PseudoPrefabSOCategory("CannonMaterial")]
        [SerializeField] public PseudoPrefabSO modelMatSO;
        [HideInInspector]
        [SerializeField] public Transform baseModelParent;
        [HideInInspector]
        [SerializeField] public Quaternion baseModelRotation = Quaternion.Euler(0, 180, 0);
        [HideInInspector]
        [SerializeField] public Transform swivelModelParent;
        [HideInInspector]
        [SerializeField] public Quaternion swivelModelRotation = Quaternion.Euler(0, 180, 0);
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO targetSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO targetMatSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO targetAnimatorSO;
        [HideInInspector]
        [SerializeField] public Transform targetParent;
        [HideInInspector]
        [SerializeField] public Vector3 targetScale = Vector3.one * -0.9421963f;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO runPuffSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO matSmokeSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO matConfettiSO;
        [SerializeField] public GameObject button;
    }
}