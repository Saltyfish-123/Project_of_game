using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowAbleObject : MonoBehaviour
{
    [SerializeField] private int id_self;
    public void ShowInfo()
    {
        if (id_self == 0)
        {
            Debug.Log("–≈œ¢¥ÌŒÛ");
        }
        else
        {
            ShowInform.ShowObjectInfo(id_self);
        }
    }
}
