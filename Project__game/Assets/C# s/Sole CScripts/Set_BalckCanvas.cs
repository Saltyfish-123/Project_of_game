using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set_BalckCanvas : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void ChangeBlackBool()
    {
        _animator.SetBool("Black Bool",true);
    }
}
