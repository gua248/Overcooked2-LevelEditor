using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabIngredientSprayStub : PseudoPrefabStub
    {
        [PseudoPrefabSOCategory("Ingredient")]
        [SerializeField] public PseudoPrefabSO orderPrefabSO;
    }
}