using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace LevelEditor
{
    public class SetupCustomCounter : SetupCustomPrefab
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomCounterStub counterStub = (SetupCustomCounterStub)stub;
            GameObject progressUIPrefab = counterStub.progressUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(counterStub.progressUIPrefabSO);
            GameObject fireEffectPrefab = counterStub.fireEffectPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(counterStub.fireEffectPrefabSO);
            GameObject chopPFX = counterStub.chopPFXSO == null ? null : PseudoPrefabManager.LoadAsset(counterStub.chopPFXSO);
            GameObject tutorialIconUI = counterStub.tutorialIconUISO == null ? null : PseudoPrefabManager.LoadAsset(counterStub.tutorialIconUISO);

            Flammable flammable = GetComponent<Flammable>();
            Workstation workstation = GetComponent<Workstation>();
            ButtonHoverIcon buttonHoverIcon = GetComponent<ButtonHoverIcon>();

            if (flammable != null)
            {
                flammable.m_progressUIPrefab = progressUIPrefab.GetComponent<ProgressUIController>();
                flammable.m_fireEffectPrefab = fireEffectPrefab;
            }

            if (workstation != null)
            {
                workstation.m_chopPFX = chopPFX;
            }

            if (buttonHoverIcon != null)
            {
                typeof(ButtonHoverIcon)
                    .GetField("m_iconPrefab", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(buttonHoverIcon, tutorialIconUI);
            }
        }

        public override void Clear()
        {
            base.Clear();
            Flammable flammable = GetComponent<Flammable>();
            Workstation workstation = GetComponent<Workstation>();
            ButtonHoverIcon buttonHoverIcon = GetComponent<ButtonHoverIcon>();

            if (flammable != null)
            {
                flammable.m_progressUIPrefab = null;
                flammable.m_fireEffectPrefab = null;
            }

            if (workstation != null)
            {
                workstation.m_chopPFX = null;
            }

            if (buttonHoverIcon != null)
            {
                typeof(ButtonHoverIcon)
                    .GetField("m_iconPrefab", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(buttonHoverIcon, null);
            }
        }
    }
}