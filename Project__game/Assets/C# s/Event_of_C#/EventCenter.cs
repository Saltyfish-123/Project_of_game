using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : MonoBehaviour
{
    private EventCenter Instance;
    private void Awake()
    {
        Instance = this;
    }//ตฅภýปฏ

    public static event Action onEnterTriggerArea;

    public void DoEnterevent()
    {
         if (onEnterTriggerArea!=null)
        {
            onEnterTriggerArea();
        }
    }
}
