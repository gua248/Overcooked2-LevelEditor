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

        private void Rotate()
        {
            Mesh newMesh = new Mesh();
            newMesh.name = mesh.name;
            newMesh.vertices = mesh.vertices;
            newMesh.triangles = mesh.triangles;
            newMesh.normals = mesh.normals;

            Vector2[] uvs = mesh.uv;
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            // Rotate around the center (0.5, 0.5)
            Vector2 center = new Vector2(0.5f, 0.5f);
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = (Vector2)(rotation * (uvs[i] - center)) + center;
            }
            newMesh.uv = uvs;

            GetComponent<MeshFilter>().sharedMesh = newMesh;
        }

        private void OnValidate()
        {
            Rotate();
        }

        private void Start()
        {
            if (Application.isPlaying)
                Rotate();
        }
    }
}