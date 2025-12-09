using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabTeleportal : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabTeleportalStub teleportalStub = (PseudoPrefabTeleportalStub)stub;
            Material material = PseudoPrefabManager.LoadAsset<Material>(teleportalStub.materialSOs[(int)teleportalStub.portalColor]);
            childGameObject.transform.Find("Portal/m_wizard_portal_02").GetComponent<Renderer>().material = material;
            Material material1 = PseudoPrefabManager.LoadAsset<Material>(teleportalStub.pfxMaterialSOs[(int)teleportalStub.portalColor]);
            Color color = teleportalStub.pfxColors[(int)teleportalStub.portalColor];
            if (teleportalStub.portalColor == PseudoPrefabTeleportalStub.PortalColor.Blue || teleportalStub.portalColor == PseudoPrefabTeleportalStub.PortalColor.Tree)
            {
                material1 = new Material(material1);
                material1.SetColor("_colour", color);
            }
            childGameObject.transform.Find("Portal/ms_pfx_Teleport_1").GetComponent<Renderer>().material = material1;

            GameObject portal = childGameObject.transform.Find("Portal").gameObject;
            var colorOverLifetime = portal.transform.Find("pfx_EnergyOrbs").GetComponent<ParticleSystem>().colorOverLifetime;
            var newGradient = colorOverLifetime.color;
            var newColorKeys = newGradient.gradient.colorKeys;
            newColorKeys[1] = new GradientColorKey(color, newColorKeys[1].time);
            newColorKeys[2] = new GradientColorKey(color, newColorKeys[2].time);
            newGradient.gradient.SetKeys(newColorKeys, newGradient.gradient.alphaKeys);
            colorOverLifetime.color = newGradient;

            if (teleportalStub.doubleSided)
            {
                GameObject portalBack = Instantiate(portal, portal.transform.parent);
                portalBack.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                portalBack.transform.Find("pfx_EnergyOrbs").gameObject.SetActive(false);
                portalBack.transform.Find("ms_pfx_Teleport_1").gameObject.SetActive(false);
            }
        }

        public override void LateSetup()
        {
            PseudoPrefabTeleportalStub teleportalStub = (PseudoPrefabTeleportalStub)stub;
            childGameObject.GetComponent<Teleportal>().m_exitPortal = teleportalStub.exitPortal.GetComponent<PseudoPrefab>().childGameObject;
        }
    }
}