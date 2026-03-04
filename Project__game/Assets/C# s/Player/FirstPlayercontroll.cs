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
    [SerializeField] private float flySpeed = 10f;
    [SerializeField] private float flyVerticalSpeed = 5f;
    [SerializeField] private float doubleClickTime = 0.3f; // 双击判定时间

    private float verticalVelocity;
    private bool isFlying = false;
    private float lastJumpTime = -10f; // 记录上一次跳跃的时间

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
        Vector3 movedir = transform.right * shuiping + transform.forward * shuzhi;

        // 检测双击空格
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.time - lastJumpTime < doubleClickTime)
            {
                isFlying = !isFlying; // 切换飞行状态
                verticalVelocity = 0; // 切换时重置垂直速度！！！
            }
            lastJumpTime = Time.time;
        }

        if (isFlying)
        {
            // 飞行模式：禁用重力，用空格上升，左Ctrl下降
            float flyVerticalInput = 0;
            if (Input.GetKey(KeyCode.Space))
                flyVerticalInput = flyVerticalSpeed;
            if (Input.GetKey(KeyCode.LeftControl))
                flyVerticalInput = -flyVerticalSpeed;

            Vector3 flyMove = movedir.normalized * flySpeed + Vector3.up * flyVerticalInput;
            controller.Move(flyMove * Time.deltaTime);
        }
        else
        {
            // 行走跳跃模式：保留原有重力
            if (controller.isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                    verticalVelocity = jumpForce;
                else
                    verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            Vector3 move = movedir * movespeed + Vector3.up * verticalVelocity;
            controller.Move(move * Time.deltaTime);
        }
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