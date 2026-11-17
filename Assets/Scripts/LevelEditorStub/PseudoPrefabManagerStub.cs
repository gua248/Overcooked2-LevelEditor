using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [DefaultExecutionOrder(-100)]
    public class PseudoPrefabManagerStub : MonoBehaviour
    {
        [SerializeField] public LevelInfoSO levelInfo;

        [HideInInspector]
        [SerializeField] public PseudoPrefabSO GoSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO ReadySO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO TutorialSplashSO;
        [SerializeField] public GameObject FlowManagerGO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO RecipeUISO;
        [SerializeField] public GameObject RecipeUIGO;

        [HideInInspector]
        [SerializeField] public PseudoPrefabSO RoundResultsSO;
        [SerializeField] public GameObject AudioManagerGO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO[] PFXSOs;
        [SerializeField] public GameObject PlayerSwitchingManagerGO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO[] PlayerColourSOs;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO PlayerBlackCatSO;
        [HideInInspector]
        [SerializeField] public PseudoPrefabSO GameMetaEnvironmentSO;
        [SerializeField] public GameObject BootstrapManagerGO;

        [SerializeField] public GameObject KillPlaneGO;

        [PseudoPrefabSOCategory("ConfigTemplate")]
        [SerializeField] public PseudoPrefabSO configTemplateSO;

        [SerializeField] public PseudoPrefabSO hordeModeUISO;
        [SerializeField] public PseudoPrefabSO hordeWaveNumberUISO;

        private void Awake()
        {
            // entry for bepinex plugin patch
        }

        private void OnDestroy()
        {
            // entry for bepinex plugin patch
        }
    }
}
