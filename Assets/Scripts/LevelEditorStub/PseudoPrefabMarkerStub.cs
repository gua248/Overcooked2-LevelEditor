using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabMarkerStub : PseudoPrefabStub
    {
        [PseudoPrefabSOCategory("MarkerMaterial")]
        [SerializeField] public PseudoPrefabSO activeLightstripMaterialSO;
        [PseudoPrefabSOCategory("MarkerMaterial")]
        [SerializeField] public PseudoPrefabSO inactiveLightstripMaterialSO;
    }
}