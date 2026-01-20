using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/LevelInfoSO")]
    public class LevelInfoSO : ScriptableObject
	{
        [SerializeField] public string levelName;
        [SerializeField] public string levelNameZH;
        [SerializeField] public Sprite screenshot;
        [SerializeField] public string sceneName;
        
        [SerializeField] public ScriptableObject[] recipes;
        [SerializeField] public bool useScore2 = false;
        [SerializeField] public int debugRecipeCount;

        [SerializeField] public bool disableDynamicParenting = true;

        [SerializeField] public LevelConfigSetupPerPlayerCountSO config_1p;
        [SerializeField] public LevelConfigSetupPerPlayerCountSO config_2p;
        [SerializeField] public LevelConfigSetupPerPlayerCountSO config_3p;
        [SerializeField] public LevelConfigSetupPerPlayerCountSO config_4p;

        [SerializeField] public string[] dependencies;
    }
}