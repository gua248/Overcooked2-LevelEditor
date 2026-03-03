using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class FloatingObjectShakeStub : Stub
    {
        [SerializeField] public float verticalAmplitude = 0.1f; // 垂直浮动幅度
        [SerializeField, Range(0.1f, 3f)] public float verticalFrequency = 1f; // 垂直浮动频率

        [SerializeField] public float rotationAmplitude = 5f; // 旋转浮动幅度
        [SerializeField, Range(0.1f, 3f)] public float rotationFrequency = 0.8f; // 旋转浮动频率

        [SerializeField] public float horizontalMovement = 0.1f; // 水平微小移动
        [SerializeField] public Vector2 horizontalFrequency = new Vector2(0.5f, 0.7f); // 水平移动频率

        [SerializeField, Range(0f, 1f)] public float randomness = 0.3f; // 随机性系数

        void Start()
        {
            verticalAmplitude = Mathf.Max(0f, verticalAmplitude);
            rotationAmplitude = Mathf.Max(0f, rotationAmplitude);
        }
    }
}