using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabAttachingFoodSpawnerStub : PseudoPrefabStub
    {
        [SerializeField] public bool spawnInOrder = true;
        [SerializeField] public PseudoPrefabSO[] attachmentPrefabSOs;
        [SerializeField] public float[] weights;
        [SerializeField] public float triggerTime = 5f;
        [SerializeField] public bool triggerAtStart = true;
    }
}
