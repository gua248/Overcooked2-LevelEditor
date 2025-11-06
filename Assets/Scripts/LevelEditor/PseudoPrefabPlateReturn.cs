using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
	public class PseudoPrefabPlateReturn : PseudoPrefab
	{
        private GameObject dirtyPlateStack;
        private GameObject cleanPlateStack;

        public override void Setup()
        {
            PseudoPrefabPlateReturnStub plateReturnStub = (PseudoPrefabPlateReturnStub)stub;

            dirtyPlateStack = PseudoPrefabManager.LoadAsset(plateReturnStub.dirtyPlateStackSO);
            cleanPlateStack = PseudoPrefabManager.LoadAsset(plateReturnStub.cleanPlateStackSO);

            PlateReturnStation plateReturnStation = childGameObject.GetComponent<PlateReturnStation>();
            plateReturnStation.m_stackPrefab = plateReturnStub.returnClean ? cleanPlateStack : dirtyPlateStack;
        }
    }
}