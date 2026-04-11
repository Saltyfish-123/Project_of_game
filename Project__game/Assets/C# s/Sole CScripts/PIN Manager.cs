using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PINManager : MonoBehaviour
{

    [SerializeField] private List<NPC_Informs> NPC_s = new List<NPC_Informs>();
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Resert_Location;

    public static List<NPC_Informs> NPC_outs;

    //用来记录当前的NPC
    private int currentNPCIndex = 0;
    private List<NPC_Informs> sortedNPCs = new List<NPC_Informs>();
    private void Awake()
    {
        NPC_outs = NPC_s;

        // 按照NPC_group进行排序
        sortedNPCs = new List<NPC_Informs>(NPC_s);
        sortedNPCs.Sort((a, b) => a.NPC_group.CompareTo(b.NPC_group));
    }
    [System.Serializable]
    public class NPC_Informs
    {
        public string name;
        public int NPC_group;//表示NPC的顺序
        public int Need_PIN;
        public GameObject model_NPC;
        public UnityEvent NPc_event;//这里的事件是在NPC对话完之后发生的事情
        //发生之前的事件要在对话那里设置
    }

    public void TurnNextNPC()
    {
        sortedNPCs[currentNPCIndex].NPc_event?.Invoke();

        if (sortedNPCs.Count == 0)
        {
            Debug.LogWarning("没有NPC可跳转");
            return;
        }

        // 停用当前NPC
        if (currentNPCIndex >= 0 && currentNPCIndex < sortedNPCs.Count)
        {
            if (sortedNPCs[currentNPCIndex].model_NPC != null)
            {
                sortedNPCs[currentNPCIndex].model_NPC.SetActive(false);
            }
        }

        // 移动到下一个NPC
        currentNPCIndex++;

        // 激活下一个NPC
        if (currentNPCIndex >= 0 && currentNPCIndex < sortedNPCs.Count)
        {
            if (Resert_Location == null)
            {
                Debug.Log("重置的位置没有设置");
            }

            Player.transform.position = Resert_Location.transform.position;
            Player.transform.rotation = Resert_Location.transform.rotation;
            //重置玩家位置防止直接触发对话

            NPC_Informs nextNPC = sortedNPCs[currentNPCIndex];

            // 激活模型
            if (nextNPC.model_NPC != null)
            {
                nextNPC.model_NPC.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"NPC {nextNPC.name} 的模型为空");
            }
        }
    }
}