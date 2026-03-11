using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform NpcTransform;

    private bool hasSpoken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            other.gameObject.GetComponent<DialogueManger>().DialogueStart(dialogueStrings, NpcTransform);
            hasSpoken = true;
        }
    }
}
[System.Serializable]
public class dialogueString
{
    public string text;//npc says
    public bool isEnd;//judge if is the final

    [Header("Branch")]
    public bool isQuestion;
    public string answerOptiong1;
    public string answerOptiong2;
    public int option1IndexJump;//Index of Jump
    public int option2IndexJump;

    [Header("Trigger Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;

}