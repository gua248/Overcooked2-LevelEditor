using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace LevelEditorStub
{
	[CreateAssetMenu(menuName = "LevelEditor/LevelSetInfoSO")]
	public class LevelSetInfoSO : ScriptableObject
	{
		[SerializeField] public string levelSetName;
		[SerializeField] public string levelSetNameZH;
		[SerializeField] public string author;
        [SerializeField] public string uid;
		[SerializeField] public string version;
        [SerializeField] public LevelInfoSO[] levelInfos;

        [SerializeField, HideInInspector]
        private string lastVersion;

        [SerializeField, HideInInspector]
        private string baseUID; // 对象诞生时生成的唯一底码

        private void OnValidate()
        {
            // 只有当 version 改变时更新
            if (version != lastVersion && !string.IsNullOrEmpty(baseUID))
            {
                RefreshUID();
                lastVersion = version;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        private void Reset()
        {
            // 生成该物体终身唯一的底码
            baseUID = Guid.NewGuid().ToString();
            version = string.Empty;
            lastVersion = string.Empty;
            RefreshUID();
        }

        private void RefreshUID()
        {
            // 结合 baseUID 和 version 生成最终 UID
            string raw = baseUID + version.ToString();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
                uid = new Guid(hashBytes).ToString();
            }
        }
    }
}