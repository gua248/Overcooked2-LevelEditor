using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabTerminal : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabTerminalStub terminalStub = (PseudoPrefabTerminalStub)stub;
            childGameObject.GetComponent<Terminal>().m_pilotableObject = terminalStub.pilotableObject.GetComponent<PilotMovement>();
            foreach (var component in childGameObject.GetComponents<ForwardTriggerToTarget>())
            {
                DestroyImmediate(component);
            }
        }
    }
}