using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelEditor
{
    public class SetupCustomTray : SetupCustomUtensil
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomTrayStub trayStub = (SetupCustomTrayStub)stub;
            GameObject sludge = trayStub.sludgeSO == null ? null : PseudoPrefabManager.LoadAsset(trayStub.sludgeSO);

            TrayCosmeticDecisions trayCosmeticDecisions = GetComponent<TrayCosmeticDecisions>();
            Tray tray = GetComponent<Tray>();

            if (trayCosmeticDecisions != null)
                trayCosmeticDecisions.m_noMatchingRecipePrefab = sludge;
            if (tray != null && !trayStub.slots.IsEmpty())
                tray.m_slots = trayStub.slots.Select(x => RecipeHelper.BuildOrderToPrefabLookup(x)).ToArray();
        }

        public override void Clear()
        {
            base.Clear();
            TrayCosmeticDecisions trayCosmeticDecisions = GetComponent<TrayCosmeticDecisions>();
            Tray tray = GetComponent<Tray>();

            if (trayCosmeticDecisions != null)
                trayCosmeticDecisions.m_noMatchingRecipePrefab = null;
            if (tray != null)
            {
                if (!tray.m_slots.IsEmpty())
                {
                    foreach (var slot in tray.m_slots)
                        DestroyImmediate(slot);
                }
                tray.m_slots = new OrderToPrefabLookup[0];
            }
        }
    }
}