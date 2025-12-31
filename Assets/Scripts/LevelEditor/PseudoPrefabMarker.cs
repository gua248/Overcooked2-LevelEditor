using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabMarker : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabMarkerStub markerStub = (PseudoPrefabMarkerStub)stub;
            greenLightstripMaterial = PseudoPrefabManager.LoadAsset<Material>(markerStub.activeLightstripMaterialSO);
            inactiveLightstripMaterial = PseudoPrefabManager.LoadAsset<Material>(markerStub.inactiveLightstripMaterialSO);
            redLightstripMaterial = new Material(greenLightstripMaterial);
            redLightstripMaterial.SetColor("_Color", new Color32(255, 30, 30, 255));
            redLightstripMaterial.SetColor("_EmissionColor", Color.red);
            whiteLightstripMaterial = new Material(greenLightstripMaterial);
            whiteLightstripMaterial.SetColor("_Color", new Color32(100, 100, 100, 255));
            whiteLightstripMaterial.SetColor("_EmissionColor", Color.white);
            lightPFX = childGameObject.transform.Find("glow (2)").GetComponent<ParticleSystem>();
            marker = childGameObject.transform.Find("m_sk_joystick_marker_light").GetComponent<Renderer>();
            OnTrigger("DeactivateMarker");
        }

        public void OnTrigger(string trigger)
        {
            if (trigger == greenTrigger)
            {
                lightPFX.gameObject.SetActive(true);
                var main = lightPFX.main;
                main.startColor = (Color)new Color32(0, 255, 65, 187);
                lightPFX.Stop();
                lightPFX.Clear();
                lightPFX.Play();
                marker.materials = new Material[] { greenLightstripMaterial };
            }
            else if (trigger == deactivateTrigger)
            {
                lightPFX.gameObject.SetActive(false);
                marker.materials = new Material[] { inactiveLightstripMaterial };
            }
            else if (trigger == redTrigger)
            {
                lightPFX.gameObject.SetActive(true);
                var main = lightPFX.main;
                main.startColor = (Color)new Color32(255, 0, 0, 187);
                lightPFX.Stop();
                lightPFX.Clear();
                lightPFX.Play();
                marker.materials = new Material[] { redLightstripMaterial };
            }
            else if (trigger == whiteTrigger)
            {
                lightPFX.gameObject.SetActive(true);
                var main = lightPFX.main;
                main.startColor = (Color)new Color32(220, 220, 220, 187);
                lightPFX.Stop();
                lightPFX.Clear();
                lightPFX.Play();
                marker.materials = new Material[] { whiteLightstripMaterial };
            }
        }

        private Material whiteLightstripMaterial;
        private Material redLightstripMaterial;
        private Material greenLightstripMaterial;
        private Material inactiveLightstripMaterial;
        private ParticleSystem lightPFX;
        private Renderer marker;
        const string whiteTrigger = "WhiteMarker";
        const string greenTrigger = "GreenMarker";
        const string deactivateTrigger = "DeactivateMarker";
        const string redTrigger = "RedMarker";
    }
}