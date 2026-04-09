using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.ComponentModel;
using Unity.VisualScripting;

public class DialogueManger : MonoBehaviour
{
    [SerializeField] private GameObject Player_Camera; 
    [SerializeField] private GameObject dialogueParent;//panel of Dialogue
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [SerializeField] private float typingSpeed = 0.05f;//print Speed
    [SerializeField] private float turnSpeed = 2f;//the speed of turn to the NPC

    [SerializeField] FirstPlayercontroll FirstPlayercontroll;
    private List<dialogueString> dialogueList;
    [SerializeField] private FirstPlayercontroll player;

    [Header("Player")]
    private Transform playerCamera;

    private int currentDialogueIndex = 0;

    private void Start()
    {
        dialogueParent.SetActive(false);
        playerCamera = Player_Camera.transform;
    }



    public void DialogueStart(List<dialogueString> textToPrint, Transform NPC)
    {
        dialogueParent.SetActive(true);
        // firstCameracontrool.enable = false;这一步是在禁用第一人称控制器

        if (FirstPlayercontroll != null)
        {
            FirstPlayercontroll.enabled = false;
        }


        Cursor.lockState = CursorLockMode.None;//不禁用鼠标
        Cursor.visible = true;

        StartCoroutine(TurnCameraTowardsNPC(NPC));

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();//禁用按钮

        StartCoroutine(PrintDialogue());
    }

    private void DisableButtons()
    {
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<Text>().text = "No Option";
        option2Button.GetComponentInChildren<Text>().text = "No Option";
    }

    private bool optionSlected = false;

    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);

        //这里要启用一下第一人称控制器的组件
        if (FirstPlayercontroll != null)
        {
            FirstPlayercontroll.enabled = true;
        }

        //player.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(0, 0, 0);

        Player_Camera.transform.localScale = Vector3.zero;
        Player_Camera.transform.localRotation = Quaternion.Euler(0, 0, 0);

        Cursor.lockState = CursorLockMode.Locked;//不禁用鼠标
        Cursor.visible = false;
    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSlected = true;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }


    private IEnumerator TurnCameraTowardsNPC(Transform NPC)
    {
        Quaternion startrotation = playerCamera.rotation;
        Quaternion targetrotation = Quaternion.LookRotation(NPC.position - playerCamera.position);

        float elaspedTime = 0f;//视角转向的时间
        while (elaspedTime < 1f)
        {
            playerCamera.rotation = Quaternion.Slerp(startrotation, targetrotation, elaspedTime);
            elaspedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }

        playerCamera.rotation = targetrotation;
    }

    private IEnumerator PrintDialogue()
    {
        while (currentDialogueIndex < dialogueList.Count)
        {
            dialogueString line = dialogueList[currentDialogueIndex];

            line.startDialogueEvent?.Invoke();
            //?. 是一个安全调用语法，避免空值

            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));

                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<Text>().text = line.answerOptiong1;
                option2Button.GetComponentInChildren<Text>().text = line.answerOptiong2;

                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSlected);
            }
            else
            {
                yield return StartCoroutine(TypeText(line.text));
            }

            line.endDialogueEvent?.Invoke();

            optionSlected = false;
        }
        DialogueStop();
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
        }

        if (dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
        }

        currentDialogueIndex++;
    }

}