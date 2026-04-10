using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using TMPro;

public class ShowInform : MonoBehaviour
{
    public static ShowInform Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
        ChangeEvent.AddListener(Show_Inform);
    }
    //单例化

    public static int PIN_self;

    [Header("对应赋值")]
    [SerializeField] private ObjectsManger objectsManger;
    [SerializeField] private TMP_Text Name_OfObject;
    [SerializeField] private TMP_Text Introduction_Ofobject;
    [SerializeField] private GameObject Image_Ofobject;
    [SerializeField] private Button Chose_theObject;

    [SerializeField] private GameObject Panel_OfShow;
    //信息设置

    public UnityEvent ChangeEvent;
    
    private void Show_Inform()
    {

        if (objectsManger == null)
        {
            Debug.Log("未赋值脚本信息");
        }
        if (PIN_self == 0)
        {
            Debug.Log("PIN值错误");
        }

        Panel_OfShow.SetActive(true);
        //显示界面

        Object_inform Show_inform = objectsManger.GetInformFromId(PIN_self);
        Name_OfObject.text = Show_inform.name;
        Introduction_Ofobject.text = Show_inform.description;
        UnityEngine.UI.Image imageComponent = Image_Ofobject.GetComponent<UnityEngine.UI.Image>();
        imageComponent.sprite = Show_inform.image;
        //修改显示的信息
    }

    public void TriggerEvent(int num)
    {
        PIN_self = num;
        ChangeEvent?.Invoke();
    }

    public static void ShowObjectInfo(int id)
    {
        if (Instance != null)
        {
            Instance.TriggerEvent(id);
        }
        else
        {
            Debug.Log("ShowInform实例未找到");
        }
    }
}

