using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
	public class PseudoPrefabPlateReturnStub : PseudoPrefabStub {

		[SerializeField] public PseudoPrefabSO dirtyPlateStackSO;
		[SerializeField] public PseudoPrefabSO cleanPlateStackSO;
        [SerializeField] public bool returnClean;

	}
}
