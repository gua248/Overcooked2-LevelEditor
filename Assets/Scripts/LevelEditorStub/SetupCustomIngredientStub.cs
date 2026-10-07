using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [DefaultExecutionOrder(-99)]
    public class SetupCustomIngredientStub : SetupCustomPrefabStub
    {
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO ingredientContentUIPrefabSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO throwParticleSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO progressUIPrefabSO;

        private void Awake()
        {
            // entry for bepinex plugin patch
        }
    }
}
