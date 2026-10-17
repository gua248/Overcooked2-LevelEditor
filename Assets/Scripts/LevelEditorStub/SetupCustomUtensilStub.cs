using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [DefaultExecutionOrder(-99)]
    public class SetupCustomUtensilStub : SetupCustomPrefabStub
    {
        [SerializeField] public PseudoPrefabSO ingredientContentUIPrefabSO;
        [SerializeField] public PseudoPrefabSO sludgeSO;
        [SerializeField] public PseudoPrefabSO spawnEffectSO;
        [SerializeField] public PseudoPrefabSO progressUIPrefabSO;
    }
}

