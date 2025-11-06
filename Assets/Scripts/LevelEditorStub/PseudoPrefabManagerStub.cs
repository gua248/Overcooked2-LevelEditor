using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [DefaultExecutionOrder(-100)]
    public class PseudoPrefabManagerStub : MonoBehaviour {

        [SerializeField] public PseudoPrefabSO GoSO;
        [SerializeField] public PseudoPrefabSO ReadySO;
        [SerializeField] public PseudoPrefabSO TutorialSplashSO;
        [SerializeField] public GameObject FlowManagerGO;
        [SerializeField] public PseudoPrefabSO RecipeUISO;
        [SerializeField] public GameObject RecipeUIGO;
        [SerializeField] public PseudoPrefabSO InLevelMusicSO;
        [SerializeField] public PseudoPrefabSO RoundResultsSO;
        [SerializeField] public PseudoPrefabSO[] AudioDirectorySOs;
        [SerializeField] public GameObject AudioManagerGO;
        [SerializeField] public PseudoPrefabSO[] PFXSOs;
        [SerializeField] public GameObject PlayerSwitchingManagerGO;
        [SerializeField] public PseudoPrefabSO[] PlayerColourSOs;
        [SerializeField] public PseudoPrefabSO PlayerBlackCatSO;
        [SerializeField] public PseudoPrefabSO GameMetaEnvironmentSO;
        [SerializeField] public GameObject BootstrapManagerGO;

        [SerializeField] public PseudoPrefabSO configTemplateSO;
        [SerializeField] public LevelInfoSO levelInfo;

        private void Awake()
        {
            // entry for bepinex plugin patch
        }
    }
}
