using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
	public class PseudoPrefabCookingUtensilStub : PseudoPrefabStub {

        [SerializeField] public int capacity;
		[SerializeField] public PseudoPrefabSO[] allowedIngredientSOs = new PseudoPrefabSO[0];
	}
}
