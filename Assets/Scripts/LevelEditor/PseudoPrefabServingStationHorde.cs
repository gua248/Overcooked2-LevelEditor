using GameModes.Horde;
using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class PseudoPrefabServingStationHorde : PseudoPrefabServingStation
    {
        public override void Setup()
        {
            base.Setup();
            PseudoPrefabServingStationHordeStub servingStationHordeStub = (PseudoPrefabServingStationHordeStub)stub;
            HordeTargetCosmeticDecisions hordeTargetCosmeticDecisions = childGameObject.GetComponent<HordeTargetCosmeticDecisions>();
            Transform orderUITransform = hordeTargetCosmeticDecisions.m_orderUITransform;
            orderUITransform.localPosition = orderUITransform.localPosition + servingStationHordeStub.orderUIOffset;
            Transform healthUITransform = hordeTargetCosmeticDecisions.m_healthUITransform;
            healthUITransform.localPosition = healthUITransform.localPosition + servingStationHordeStub.healthUIOffset;
        }
    }
}