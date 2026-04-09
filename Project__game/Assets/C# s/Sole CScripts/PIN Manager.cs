using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PINManager : MonoBehaviour
{
    [SerializeField] private List<NPC_Informs>NPC_s = new List<NPC_Informs>();

}
[System.Serializable]
public class NPC_Informs
{
    public string name;
    public int PIN_nums;
    public GameObject model_NPC;
    public string orders;
}
