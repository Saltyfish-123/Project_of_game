using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayercontroll : MonoBehaviour
{
    public float movespeed;
    public float mousechangespeed;
    public CharacterController controller;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpForce = 8f;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.tag = "Player";
        GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Playermove();
            Shijiaoyidong();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(1) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Playermove()
    {
        float shuiping = Input.GetAxis("Horizontal");
        float shuzhi = Input.GetAxis("Vertical");
        Vector3 movedirtion = transform.right * shuiping + transform.forward * shuzhi;

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                verticalVelocity = jumpForce;
            else
                verticalVelocity = -2f;
        }
        else
            verticalVelocity -= gravity * Time.deltaTime;

        Vector3 move = movedirtion * movespeed + Vector3.up * verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void Shijiaoyidong()
    {
        float Xmouse = Input.GetAxis("Mouse X");
        float Ymouse = Input.GetAxis("Mouse Y");
        float Xjiaodu = transform.eulerAngles.y + Xmouse * Time.deltaTime * mousechangespeed;
        float Yjiaodu = transform.eulerAngles.x + Ymouse * Time.deltaTime * mousechangespeed * -1;
        transform.eulerAngles = new Vector3(Yjiaodu, Xjiaodu, 0f);
    }
}
