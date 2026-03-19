using UnityEngine;
using Cinemachine;
public class DimensionSwitcher : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera cam3D;
    public CinemachineVirtualCamera cam2D;

    bool is2D = false;
    void Start()
    {
        //向本体和子物体发送消息，通知它们切换到3D模式
        BroadcastMessage("SwitchTo3D");
    }
    void SwitchTo2D()
    {
        //优先级高的夺取控制权
        cam2D.Priority = 11;
        cam3D.Priority = 10;
    }

    void SwitchTo3D()
    {
        //优先级高的夺取控制权
        cam3D.Priority = 11;
        cam2D.Priority = 10;
    }

    void Update()
    {
        //切换按键Tab
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            is2D = !is2D;
            if (is2D)
                BroadcastMessage("SwitchTo2D");
            else
                BroadcastMessage("SwitchTo3D");
        }

    }
}
