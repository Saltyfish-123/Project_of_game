using UnityEngine;

public class PlayerWoodInteraction : MonoBehaviour
{
    [Header("木板设置")]
    public float woodSpeedInfluence = 0.5f;  // 木板速度对玩家的影响系数
   
    private CharacterController characterController;
    private Rigidbody rb;
    private GameObject currentWood = null;
    private Vector3 woodVelocity = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 如果玩家站在木板上，跟随木板移动
        if (currentWood != null)
        {
            WoodMovement woodMove = currentWood.GetComponent<WoodMovement>();
            if (woodMove != null)
            {
                // 获取木板移动方向的速度
                Vector3 woodMovement = woodMove.moveDirection * woodMove.moveSpeed;

                // 将木板速度部分传递给玩家
                Vector3 playerMovement = woodMovement * woodSpeedInfluence;

                // 如果使用CharacterController
                if (characterController != null)
                {
                    characterController.Move(playerMovement * Time.deltaTime);
                }
                // 如果使用Rigidbody
                else if (rb != null)
                {
                    rb.velocity = new Vector3(playerMovement.x, rb.velocity.y, playerMovement.z);
                }
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        HandleWoodCollision(hit.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleWoodCollision(collision.gameObject);
    }

    void HandleWoodCollision(GameObject other)
    {
        if (other.CompareTag("Wood"))
        {
            currentWood = other;

            // 玩家站在木板上
            Debug.Log("Player standing on moving wood");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wood"))
        {
            if (currentWood == collision.gameObject)
            {
                currentWood = null;
            }
        }
    }

   
}
