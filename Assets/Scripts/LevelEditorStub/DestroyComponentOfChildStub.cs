using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [RequireComponent(typeof(PseudoPrefabStub))]
    public class DestroyComponentOfChildStub : MonoBehaviour
    {
        [SerializeField] public string[] componentNames;
    }
}