using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class SetupCustomSink : SetupCustomCounter
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomSinkStub sinkStub = (SetupCustomSinkStub)stub;
            GameObject progressUIPrefab = sinkStub.progressUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(sinkStub.progressUIPrefabSO);
            GameObject cleanPlateStack = sinkStub.cleanPlateStackSO == null ? null : PseudoPrefabManager.LoadAsset(sinkStub.cleanPlateStackSO);

            if (sinkStub.washingPart != null && sinkStub.washingPart.GetComponent<WashingStation>() != null)
                sinkStub.washingPart.GetComponent<WashingStation>().m_progressUIPrefab = progressUIPrefab.GetComponent<ProgressUIController>();
            if (sinkStub.dryingPart != null && sinkStub.dryingPart.GetComponent<PlateReturnStation>() != null)
                sinkStub.dryingPart.GetComponent<PlateReturnStation>().m_stackPrefab = cleanPlateStack;
        }

        public override void Clear()
        {
            base.Clear();
            SetupCustomSinkStub sinkStub = (SetupCustomSinkStub)stub;

            if (sinkStub.washingPart != null && sinkStub.washingPart.GetComponent<WashingStation>() != null)
                sinkStub.washingPart.GetComponent<WashingStation>().m_progressUIPrefab = null;
            if (sinkStub.dryingPart != null && sinkStub.dryingPart.GetComponent<PlateReturnStation>() != null)
                sinkStub.dryingPart.GetComponent<PlateReturnStation>().m_stackPrefab = null;
        }
    }
}