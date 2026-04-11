using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PINManager : MonoBehaviour
{

    [SerializeField]private List<NPC_Informs> NPC_s = new List<NPC_Informs>();

    public static List<NPC_Informs> NPC_outs;
    private void Awake()
    {
        NPC_outs = NPC_s;
    }
    [System.Serializable]
    public class NPC_Informs
    {
        public string name;
        public int NPC_group;//±íÊ¾NPCµÄË³Ðò
        public int Need_PIN;
        public GameObject model_NPC;
        public UnityEvent NPc_event;
    }
}