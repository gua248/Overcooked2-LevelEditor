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
            childGameObject.GetComponent<Renderer>().material = material;
        }
    }
}