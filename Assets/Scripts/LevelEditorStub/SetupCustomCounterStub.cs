using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [DefaultExecutionOrder(-99)]
    public class SetupCustomCounterStub : SetupCustomPrefabStub
    {
        [SerializeField] public PseudoPrefabSO progressUIPrefabSO;
        [SerializeField] public PseudoPrefabSO fireEffectPrefabSO;
        [SerializeField] public PseudoPrefabSO chopPFXSO;
        [SerializeField] public PseudoPrefabSO tutorialIconUISO;
    }
}
