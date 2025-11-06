using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
	[CreateAssetMenu(menuName = "LevelEditor/LevelSetInfoSO")]
	public class LevelSetInfoSO : ScriptableObject
	{
		[SerializeField] public string levelSetName;
		[SerializeField] public string levelSetNameZH;
        [SerializeField] public LevelInfoSO[] levelInfos;
	}
}