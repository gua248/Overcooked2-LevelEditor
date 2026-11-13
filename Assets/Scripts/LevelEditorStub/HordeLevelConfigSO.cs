using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/HordeLevelConfigSO", order = -1000)]
    public class HordeLevelConfigSO : ScriptableObject
    {
        [SerializeField] public int targetRepairCostMax = 100;
        [SerializeField] public HordeWaveDataSO[] hordeWaveDataSOs;
    }
}