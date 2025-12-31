using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    public class TriggerPasser : MonoBehaviour
    {
        public void OnTrigger(string _trigger)
        {
            if (m_targetObject != null)
            {
                m_targetObject.SendMessage("OnTrigger", _trigger, SendMessageOptions.DontRequireReceiver);
            }
            for (int i = 0; i < m_targetObjects.Length; i++)
            {
                if (m_targetObjects[i] != null)
                {
                    m_targetObjects[i].SendMessage("OnTrigger", _trigger, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        [SerializeField]
        public GameObject m_targetObject;
        [SerializeField]
        public GameObject[] m_targetObjects;
    }
}
