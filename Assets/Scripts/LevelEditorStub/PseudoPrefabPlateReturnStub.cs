using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
	public class PseudoPrefabPlateReturnStub : PseudoPrefabStub
	{
		[PseudoPrefabSOCategory("DirtyPlateStack")]
		[SerializeField] public PseudoPrefabSO dirtyPlateStackSO;
        [PseudoPrefabSOCategory("CleanPlateStack")]
        [SerializeField] public PseudoPrefabSO cleanPlateStackSO;
		[SerializeField] public bool returnClean;
	}
}