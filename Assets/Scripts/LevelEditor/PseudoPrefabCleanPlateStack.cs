using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
	public class PseudoPrefabCleanPlateStack : PseudoPrefab
	{
        public override void Setup()
        {
            PseudoPrefabCleanPlateStackStub stackStub = (PseudoPrefabCleanPlateStackStub)stub;
            if (PseudoPrefabManager.Instance.GameEditState == GameEditState.Edit)
            {
                GameObject platePrefab = PseudoPrefabManager.LoadAsset(stackStub.platePseudoPrefabSO);
                Stack stack = childGameObject.GetComponent<Stack>();

                List<GameObject> plates = new List<GameObject>();
                for (int i = 0; i < stackStub.plateCount; i++)
                {
                    GameObject plate = Instantiate(platePrefab, childGameObject.transform);
                    plate.GetComponent<EditorGridSnap>().enabled = false;
                    plates.Add(plate);
                }

                BoxCollider boxCollider = childGameObject.GetComponent<BoxCollider>();
                stack.RefreshStackTransforms(ref plates);
                stack.RefreshStackCollider(ref boxCollider, stackStub.plateCount);
            }
        }

        public override void SetupAfterStartSynchronising()
        {
            PseudoPrefabCleanPlateStackStub stackStub = (PseudoPrefabCleanPlateStackStub)stub;

            ServerCleanPlateStack serverCleanPlateStack = childGameObject.GetComponent<ServerCleanPlateStack>();
            if (serverCleanPlateStack != null)
                for (int i = 0; i < stackStub.plateCount; i++)
                    serverCleanPlateStack.AddToStack();
        }
    }
}
