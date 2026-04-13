using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character3 : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void StartNPC3()
    {
        animator.SetBool("Set Active" , true);        
    }
}
