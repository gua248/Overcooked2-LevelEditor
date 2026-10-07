using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/PseudoPrefabSO", order = -1000)]
    public class PseudoPrefabSO : ScriptableObject {

        [SerializeField] public string prefabName;
        [SerializeField] public string bundleName;
        [SerializeField] public string assetPath;
        [SerializeField] public Object asset;

        private void OnValidate()
        {
            assetPath = assetPath.Trim('"').Replace("\\", "/");
            string prefix = @"E:/dev/test/ReversedGame/ExportedProject/";
            if (assetPath.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
            {
                assetPath = assetPath.Substring(prefix.Length);
            }
        }
    }
}
