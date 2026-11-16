using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
	public class PseudoPrefabCookingUtensilStub : PseudoPrefabStub {

        [SerializeField] public int capacity;
        [PseudoPrefabSOCategory("Ingredient")]
        [PseudoPrefabSOCategory("Recipe")]
        [SerializeField] public ScriptableObject[] allowedIngredientSOs = new PseudoPrefabSO[0];
		[SerializeField] public CookingStationType[] allowedCookingStationTypes = new CookingStationType[0];
		[SerializeField] public PseudoPrefabSO[] modelSOs = new PseudoPrefabSO[0];
		[SerializeField] public GameObject[] models = new GameObject[0];

        public enum CookingStationType
        {
            Invalid,
            Hob,
            Oven,
            DeepFatFryer,
            FirePit,
            Flamethrower,
            Barbeque,
            Invalid_FloorBurner
        }
    }
}
