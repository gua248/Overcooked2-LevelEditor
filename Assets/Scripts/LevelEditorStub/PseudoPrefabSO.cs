using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/PseudoPrefabSO")]
    public class PseudoPrefabSO : ScriptableObject {

        [SerializeField] public string prefabName;
        [SerializeField] public string bundleName;
        [SerializeField] public string assetPath;
    }
}
