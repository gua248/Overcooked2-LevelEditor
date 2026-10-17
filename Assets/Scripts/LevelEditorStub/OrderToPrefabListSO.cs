using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/OrderToPrefabListSO", order = -1000)]
    public class OrderToPrefabListSO : ScriptableObject
    {
        [SerializeField] public ScriptableObject[] recipes;
        [SerializeField] public GameObject[] models;
        [SerializeField] public PseudoPrefabSO[] modelSOs;
    }
}