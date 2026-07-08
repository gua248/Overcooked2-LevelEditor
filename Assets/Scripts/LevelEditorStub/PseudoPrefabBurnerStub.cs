using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabBurnerStub : PseudoPrefabStub
    {
        [SerializeField] public FireMode fireMode;
        [SerializeField] public float airTime;
        [SerializeField] public Vector3[] targetPositions;
        [SerializeField] public bool randomTargetOrder;
        [SerializeField] public bool hideVisual;

        public enum FireMode
        {
            Direct,
            Parabolic
        }
    }
}