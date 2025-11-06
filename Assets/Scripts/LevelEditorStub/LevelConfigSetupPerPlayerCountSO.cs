using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/LevelConfigSetupPerPlayerCountSO")]
    public class LevelConfigSetupPerPlayerCountSO : ScriptableObject
    {
        public int orderLifeTime = 181;
        public int timeBetweenOrders = 10;
        public int plateReturnTime = 10;
        public float survivalTimeMultiplier = 1f;
        public int roundTime;
        public int m_OneStarScore;
        public int m_TwoStarScore;
        public int m_ThreeStarScore;
        public int m_FourStarScore;
    }
}
