using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_True_Bool : MonoBehaviour
{
    private Animator _animator_NPC3;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("进入碰撞体");
        if (other.CompareTag("NPC3"))
        {
            _animator_NPC3 = other.GetComponent<Animator>();
            _animator_NPC3.SetBool("True Bool", true);
            Debug.Log("已经修改成功布尔值");
        }
    }
}
