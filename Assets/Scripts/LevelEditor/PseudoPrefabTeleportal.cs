using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabTeleportal : PseudoPrefab
    {
        public override void LateSetup()
        {
            PseudoPrefabTeleportalStub teleportalStub = (PseudoPrefabTeleportalStub)stub;
            childGameObject.GetComponent<Teleportal>().m_exitPortal = teleportalStub.exitPortal.GetComponent<PseudoPrefab>().childGameObject;
        }
    }
}