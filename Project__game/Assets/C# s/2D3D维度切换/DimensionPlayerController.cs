using UnityEngine;

public class DimensionPlayerController : MonoBehaviour
{
    public bool is2D;
    private void SwitchTo2D()
    {
        is2D = true;
        //切换到2D模式，调整玩家的旋转和位置
    }
    private void SwitchTo3D()
    {
        is2D = false;
        //切换到3D模式
    }

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        var h = Input.GetAxis("Horizontal");
        var v= Input.GetAxis("Vertical");
        var movement = Vector3.zero;

        if (is2D)
        {
            movement = new Vector3(h, 0, 0) * moveSpeed;
        }
        else
        {
            movement = new Vector3(v, 0, -h) * moveSpeed;
        }

        Vector3 velocity = rb.velocity;
        velocity.x = movement.x;
        if (!is2D)
        {
            velocity.z = movement.z;
        }
        rb.velocity = velocity;

        //跳跃
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }

    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
