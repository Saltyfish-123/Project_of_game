using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Click : MonoBehaviour
{
    [SerializeField] GameObject Panel_inform;
    [SerializeField] private int id_self;
    [SerializeField] private int group_self;
    [SerializeField] GameObject panel_inform;
    private void OnMouseDown()
    {
        if (!panel_inform.activeSelf)
        {
            Debug.Log("鼠标点击了下来触发了事件");
            ShowInfo();
            Panel_inform.SetActive(true);
        }
    }

    public void ShowInfo()
    {
        if (id_self == 0)
        {
            Debug.Log("信息错误");
        }
        else
        {
            ShowInform.ShowObjectInfo(id_self,group_self);
        }
    }
}
