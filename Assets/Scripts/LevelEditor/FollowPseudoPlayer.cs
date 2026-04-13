using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    public class FollowPseudoPlayer : MonoBehaviour
    {
        private FollowPseudoPlayerStub stub;
        private Transform target;
        private Vector3 relativePosition;
        private Quaternion relativeRotation;

        void Awake()
        {
            stub = GetComponent<FollowPseudoPlayerStub>();
            target = stub.playerStub.GetComponent<PseudoPrefabPlayer>().childGameObject.transform;
        }

        void Start()
        {
            if (target != null)
            {
                relativePosition = target.InverseTransformPoint(transform.position);
                relativeRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
            }
        }

        void LateUpdate()
        {
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                transform.position = target.TransformPoint(relativePosition);
                transform.rotation = target.rotation * relativeRotation;
            }
        }
    }
}