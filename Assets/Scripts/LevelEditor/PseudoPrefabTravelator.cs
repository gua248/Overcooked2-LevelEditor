using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabTravelator : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabTravelatorStub travelatorStub = (PseudoPrefabTravelatorStub)stub;
            Travelator travelator = childGameObject.GetComponent<Travelator>();
            travelator.m_speed = travelatorStub.speed;
        }
    }
}