using LevelEditor;
using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabBurner : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabBurnerStub burnerStub = (PseudoPrefabBurnerStub)stub;
            ProjectileSpawner projectileSpawner = childGameObject.GetComponent<ProjectileSpawner>();
            projectileSpawner.m_fireMode = (ProjectileSpawner.FireMode)burnerStub.fireMode;
            projectileSpawner.m_airTime = burnerStub.airTime;
            projectileSpawner.m_bUseTransformPositions = false;
            projectileSpawner.m_targetPositions = burnerStub.targetPositions;
            projectileSpawner.m_transformTargetPositions = new Transform[0];
            projectileSpawner.m_randomTargetOrder = burnerStub.randomTargetOrder;
            DestroyImmediate(childGameObject.GetComponent<TriggerTimer>());
            foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
            {
                componet.m_targetObject = childGameObject;
            }
            if (burnerStub.hideVisual)
            {
                childGameObject.transform.Find("Block").gameObject.SetActive(false);
                childGameObject.transform.Find("Mesh").gameObject.SetActive(false);
                childGameObject.transform.Find("Light").gameObject.SetActive(false);
                childGameObject.transform.Find("PFX_Fire").gameObject.SetActive(false);
                childGameObject.transform.Find("PFX_Fire (1)").gameObject.SetActive(false);
            }
            else
            {
                Transform block = childGameObject.transform.Find("Block");
                block.GetComponent<BoxCollider>().enabled = true;
                block.GetComponent<BoxCollider>().center = new Vector3(0f, 0.75f, 0f);
                block.GetComponent<BoxCollider>().size = new Vector3(1.2f, 1.5f, 1.2f);
                block.GetComponent<CapsuleCollider>().enabled = false;
            }
        }

        public override void Cleanup()
        {
            foreach (var componet in gameObject.GetComponents<TriggerOnObject>())
            {
                componet.m_targetObject = null;
            }
        }
    }
}