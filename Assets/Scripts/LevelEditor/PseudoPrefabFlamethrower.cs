using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabFlamethrower : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabFlamethrowerStub flamethrowerStub = (PseudoPrefabFlamethrowerStub)stub;
            childGameObject.GetComponent<FlamethrowerSpray>().m_cookingRate = flamethrowerStub.cookingRate;
            var collision = childGameObject.GetComponent<FlamethrowerSpray>().m_sprayEffectPrefab.GetComponent<ParticleSystem>().collision;
            collision.enabled = false;
            childGameObject.GetComponent<HeldItemMeshVisibility>().Setup(HeldItemMeshVisibility.VisState.NotCarrying);
        }
    }
}