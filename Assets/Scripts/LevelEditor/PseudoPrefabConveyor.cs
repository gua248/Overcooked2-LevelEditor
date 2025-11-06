using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabConveyor : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabConveyorStub conveyorStub = (PseudoPrefabConveyorStub)stub;
            ConveyorStation conveyorStation = childGameObject.GetComponent<ConveyorStation>();
            conveyorStation.m_conveySpeed = conveyorStub.conveySpeed;
        }
    }
}