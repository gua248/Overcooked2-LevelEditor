using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
    public class PseudoPrefabServingStation : PseudoPrefab
    {
        public override void LateSetup()
        {
            PseudoPrefabServingStationStub servingStationStub = (PseudoPrefabServingStationStub)stub;
            if (servingStationStub.plateReturn != null)
                childGameObject.GetComponent<PlateStation>().m_returnStations = new PlateReturnStation[1]
                {
                    servingStationStub.plateReturn.GetComponent<PseudoPrefab>().childGameObject.GetComponent<PlateReturnStation>()
                };
        }
    }
}