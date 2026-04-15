using UnityEngine;
/// <summary>
/// AI辅助生成
/// </summary>
public class WoodMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.back;

    [Header("物理设置")]
    public float bounceForce = 300f;    // 玩家起跳时的反弹力
    public float damping = 0.9f;        // 移动时的阻尼

    private Rigidbody rb;
    private bool isPlayerOn = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 设置刚体属性
        if (rb != null)
        {
            rb.isKinematic = true;  // 默认用运动学，避免掉落
        }
    }

    void Update()
    {
        // 基础移动
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 玩家跳上木板
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;

          

           
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 玩家离开木板
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }

   

    // 在木板上起跳
    public void ApplyBounceForce(Vector3 direction, float forceMultiplier = 1.0f)
    {
        if (rb != null)
        {
            rb.AddForce(direction * bounceForce * forceMultiplier, ForceMode.Impulse);
        }
    }
}
