using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class PseudoPrefabTeleportalStub : PseudoPrefabStub
    {
        [SerializeField] public PseudoPrefabTeleportalStub exitPortal;
        [SerializeField] public PortalColor portalColor;
        [SerializeField] public bool doubleSided = false;

        [HideInInspector]
        [SerializeField] public PseudoPrefabSO[] materialSOs;

        [HideInInspector]
        [SerializeField] public PseudoPrefabSO[] pfxMaterialSOs;

        [HideInInspector]
        [SerializeField] public Color[] pfxColors;

        public enum PortalColor
        {
            Sky,
            Blue,
            Red,
            Orange,
            Green,
            Tree,
            Purple,
            ColorfulPurple,
            ColorfulOrange,
        }
    }
}