using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
	public class PseudoPrefabDispenser : PseudoPrefab
	{
        public override void Setup()
        {
            PseudoPrefabDispenserStub dispenserStub = (PseudoPrefabDispenserStub)stub;
            PickupItemSpawner pickupItemSpawner = childGameObject.GetComponent<PickupItemSpawner>();
            pickupItemSpawner.m_itemPrefab = PseudoPrefabManager.LoadAsset(dispenserStub.spawnerItemPrefabSO);

            // set the dispenser icon
            GameObject itemPrefab = pickupItemSpawner.m_itemPrefab;
            WorkableItem workableItem = itemPrefab.GetComponent<WorkableItem>();
            ISpawnableItem spawnableItem;
            if (workableItem != null)
            {
                GameObject nextPrefab = workableItem.GetNextPrefab();
                spawnableItem = nextPrefab.RequireInterface<ISpawnableItem>();
            }
            else
            {
                spawnableItem = itemPrefab.RequireInterface<ISpawnableItem>();
            }
            SubTexture2D subTexture = spawnableItem.GetSubTexture();
            ItemCrateCosmeticDecisions itemCrateCosmeticDecisions = childGameObject.GetComponent<ItemCrateCosmeticDecisions>();
            Transform transform = childGameObject.transform.FindChildRecursive(itemCrateCosmeticDecisions.m_crateLidMeshName);
            Renderer component2 = transform.GetComponent<SkinnedMeshRenderer>();
            if (component2 == null)
            {
                component2 = childGameObject.transform.GetComponent<MeshRenderer>();
            }
            Material[] materials = component2.sharedMaterials;
            Material material = new Material(materials[itemCrateCosmeticDecisions.m_materialNumber]);
            material.mainTexture = subTexture.m_atlasTexture;
            float num = (float)subTexture.m_atlasTexture.width;
            float num2 = (float)subTexture.m_atlasTexture.height;
            float x = subTexture.m_subRect.x / num;
            float y = 1f / itemCrateCosmeticDecisions.m_uvScale.y - subTexture.m_subRect.y / num2;
            material.mainTextureOffset = new Vector2(x, y);
            float x2 = itemCrateCosmeticDecisions.m_uvScale.x * subTexture.m_subRect.width / num;
            float num3 = itemCrateCosmeticDecisions.m_uvScale.y * subTexture.m_subRect.height / num2;
            material.mainTextureScale = new Vector2(x2, 0f - num3);
            materials[itemCrateCosmeticDecisions.m_materialNumber] = material;
            component2.sharedMaterials = materials;
        }
    }
}