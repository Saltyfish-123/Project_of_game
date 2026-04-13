using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToSolePanel : MonoBehaviour
{

    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private GameObject SoleCamera;
    [SerializeField] private Animator _animator;
    private bool isSole = false;
    private void Update()
    {
        ChangeBool_Hand();
        if (!isSole && Input.GetKeyDown(KeyCode.X))
        {
            SoleCamera.SetActive(true);
            PlayerCamera.SetActive(false);
            isSole = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (isSole == true && Input.GetKeyDown(KeyCode.X))
        {
            SoleCamera.SetActive(false);
            PlayerCamera.SetActive(true);
            isSole = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ChangeBool_Hand()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _animator.SetBool("Black Bool", false);
        }

    }
}
