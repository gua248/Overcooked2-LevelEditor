using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabMeshWithMaterial : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabMeshWithMaterialStub meshStub = (PseudoPrefabMeshWithMaterialStub)stub;
            Material material = PseudoPrefabManager.LoadAsset<Material>(meshStub.materialSO);
            foreach (Renderer renderer in childGameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}