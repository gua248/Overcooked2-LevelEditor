using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetAnimatorSpeed : MonoBehaviour
{
    private void Awake()
    {
        animator.SetFloat("SpeedMultiplier", speed);
    }

    [SerializeField] private float speed = 1f;
    [SerializeField] private Animator animator;
}