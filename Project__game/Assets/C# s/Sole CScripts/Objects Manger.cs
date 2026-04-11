using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsManger : MonoBehaviour
{

    public List<Object_informs> Object_s = new List<Object_informs>();


    /// <summary>
    /// 依据输入的key值来查找对应的PIN的物品的信息
    /// </summary>
    public Object_inform GetInformFromId(int key)
    {
        foreach (var entry in Object_s) 
        {
            if (entry.id == key)
            {
                return entry.object_inform;
            }
        }
        Debug.Log("未能找到对应id的物品");
        return new Object_inform();
    }


}

[System.Serializable]

public class Object_informs
{
    public int id;
    public int group;
    public Object_inform object_inform;
}

[System.Serializable]

public struct Object_inform
{
    public string name;
    public string description;
    public Sprite image;
}