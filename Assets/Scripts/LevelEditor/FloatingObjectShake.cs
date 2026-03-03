using LevelEditorStub;
using UnityEngine;


namespace LevelEditor
{
    public class FloatingObjectShake : MonoBehaviour
    {
        private FloatingObjectShakeStub stub;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private float[] randomOffsets;
        private float timer;

        void Awake()
        {
            stub = GetComponent<FloatingObjectShakeStub>();
        }
        
        void Start()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            InitializeRandomOffsets();
        }

        void Update()
        {
            timer += TimeManager.GetDeltaTime(gameObject);
            float verticalOffset = CalculateVerticalMovement(timer);
            Quaternion rotationOffset = CalculateRotationMovement(timer);
            Vector3 horizontalOffset = CalculateHorizontalMovement(timer);
            ApplyTransformations(verticalOffset, rotationOffset, horizontalOffset);
        }

        private void InitializeRandomOffsets()
        {
            randomOffsets = new float[6];
            for (int i = 0; i < randomOffsets.Length; i++)
            {
                randomOffsets[i] = Random.Range(0f, 100f);
            }
            timer = Random.Range(0f, 100f);
        }

        private float CalculateVerticalMovement(float time)
        {
            float baseMovement = Mathf.Sin(time * stub.verticalFrequency + randomOffsets[0]) * stub.verticalAmplitude;

            // 添加次级波动
            float secondaryMovement = Mathf.Sin(time * stub.verticalFrequency * 1.7f + randomOffsets[1]) * stub.verticalAmplitude * 0.3f;

            // 添加随机性
            float randomMovement = (Mathf.PerlinNoise(time * 0.5f, randomOffsets[2]) * 2f - 1f) * stub.verticalAmplitude * stub.randomness;

            return baseMovement + secondaryMovement + randomMovement;
        }

        private Quaternion CalculateRotationMovement(float time)
        {
            // 计算X轴旋转（前后倾斜）
            float rotX = Mathf.Sin(time * stub.rotationFrequency + randomOffsets[3]) * stub.rotationAmplitude;

            // 计算Z轴旋转（左右倾斜）
            float rotZ = Mathf.Cos(time * stub.rotationFrequency * 1.3f + randomOffsets[4]) * stub.rotationAmplitude;

            // 添加随机旋转
            float randomRot = (Mathf.PerlinNoise(time * 0.3f, randomOffsets[5]) * 2f - 1f) * stub.rotationAmplitude * stub.randomness;

            // 组合旋转
            Quaternion rotation = Quaternion.Euler(
                rotX + randomRot * 0.5f,
                0f,
                rotZ + randomRot * 0.5f
            );

            return rotation;
        }

        private Vector3 CalculateHorizontalMovement(float time)
        {
            float offsetX = Mathf.Sin(time * stub.horizontalFrequency.x) * stub.horizontalMovement;
            float offsetZ = Mathf.Cos(time * stub.horizontalFrequency.y) * stub.horizontalMovement;

            return new Vector3(offsetX, 0f, offsetZ);
        }

        private void ApplyTransformations(float verticalOffset, Quaternion rotationOffset, Vector3 horizontalOffset)
        {
            Vector3 newPosition = initialPosition + Vector3.up * verticalOffset + horizontalOffset;
            Quaternion newRotation = initialRotation * rotationOffset;
            transform.localPosition = newPosition;
            transform.localRotation = newRotation;
        }
    }
}