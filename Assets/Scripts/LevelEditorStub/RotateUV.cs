using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [ExecuteInEditMode]
    public class RotateUV : MonoBehaviour
    {
        [SerializeField] private float rotationAngle = 90f;
        [SerializeField] private Mesh mesh;

        private Mesh newMesh;
        private Vector2[] sourceUVs;
        private Vector2[] newUVs;

        private void Init()
        {
            if (mesh == null) return;

            if (newMesh == null)
            {
                newMesh = new Mesh();
                newMesh.name = mesh.name;
                newMesh.vertices = mesh.vertices;
                newMesh.triangles = mesh.triangles;
                newMesh.normals = mesh.normals;

                sourceUVs = mesh.uv;
                newUVs = new Vector2[sourceUVs.Length];

                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null) meshFilter.sharedMesh = newMesh;
            }
        }

        private void Rotate()
        {
            if (newMesh == null) return;

            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            // Rotate around the center (0.5, 0.5)
            Vector2 center = new Vector2(0.5f, 0.5f);
            for (int i = 0; i < newUVs.Length; i++)
            {
                newUVs[i] = (Vector2)(rotation * (sourceUVs[i] - center)) + center;
            }
            newMesh.uv = newUVs;
        }

        private void OnValidate()
        {
            Rotate();
        }

        private void Start()
        {
            Init();
            Rotate();
        }
    }
}