using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class SetupCustomCounter : SetupCustomPrefab
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomCounterStub counterStub = (SetupCustomCounterStub)stub;
            GameObject progressUIPrefab = PseudoPrefabManager.LoadAsset(counterStub.progressUIPrefabSO);
            GameObject fireEffectPrefab = PseudoPrefabManager.LoadAsset(counterStub.fireEffectPrefabSO);
            GameObject chopPFX = PseudoPrefabManager.LoadAsset(counterStub.chopPFXSO);

            Flammable flammable = GetComponent<Flammable>();
            Workstation workstation = GetComponent<Workstation>();

            if (flammable != null)
            {
                flammable.m_progressUIPrefab = progressUIPrefab.GetComponent<ProgressUIController>();
                flammable.m_fireEffectPrefab = fireEffectPrefab;
            }

            if (workstation != null)
            {
                workstation.m_chopPFX = chopPFX;
            }
        }

        public override void LateSetup()
        {
            base.LateSetup();
        }

        public override void Clear()
        {
            base.Clear();
            Flammable flammable = GetComponent<Flammable>();
            Workstation workstation = GetComponent<Workstation>();

            if (flammable != null)
            {
                flammable.m_progressUIPrefab = null;
                flammable.m_fireEffectPrefab = null;
            }

            if (workstation != null)
            {
                workstation.m_chopPFX = null;
            }
        }
    }
}