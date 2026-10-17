using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    public class SetupCustomSinkStub : SetupCustomCounterStub
    {
        [SerializeField] public GameObject washingPart;
        [SerializeField] public GameObject dryingPart;
        [SerializeField] public PseudoPrefabSO cleanPlateStackSO;
    }
}