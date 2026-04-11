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
    [Header("检查界面")]
    [SerializeField] private GameObject CheckWorror_Panel;


    private static int get_PIN ;
    private static int get_group;

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

    public static void ShowObjectInfo(int id,int group)
    {
        get_PIN = id;
        get_group = group;
        if (Instance != null)
        {
            Instance.TriggerEvent(id);
        }
        else
        {
            Debug.Log("ShowInform实例未找到");
        }
    }

    public void CheckPIN_Button()
    {
        if (PINManager.NPC_outs == null)
        {
            Debug.LogError("NPC_outs列表未初始化");
            return;
        }
        var targetNPC = PINManager.NPC_outs.Find(npc => npc.NPC_group == get_group);

        if (targetNPC == null)
        {
            Debug.LogError($"找不到NPC_group为{get_group}的NPC");
            if (CheckWorror_Panel != null)
            {
                CheckWorror_Panel.SetActive(true);
            }
            return;
        }

        int need_PIN = targetNPC.Need_PIN;
        if (get_PIN == need_PIN)
        {
            Debug.Log("匹配成功");
            targetNPC.NPc_event?.Invoke();
        }
        else
        {
            if (CheckWorror_Panel != null)
            {
                CheckWorror_Panel.SetActive(true);
            }
            else
            {
                Debug.Log("没有给检查错误界面赋值");
            }

        }
    }
}

