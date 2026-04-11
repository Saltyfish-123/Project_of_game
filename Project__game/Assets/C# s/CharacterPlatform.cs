using UnityEngine;

public class CharacterPlatform : MonoBehaviour
{
    [Header("配置")]
    public string characterName; // 字的内容（如“定”“基”）
    public Material glowMaterial; // 发光材质（需提前准备）
    public float glowDuration = 0.5f; // 发光持续时间

    private Renderer rend;
    private Material originalMaterial;
    private bool isGlowing = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material; // 保存原始材质
        }
    }

    // 碰撞触发（进入时）
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGlowing)
        {
            // 验证踩字顺序
            bool isCorrect = StepManager.Instance.CheckStep(characterName);

            if (isCorrect)
            {
                // 发光效果
                Glow();
                // 如果是最后一步（桥），延迟跳转
                if (characterName == "桥")
                {
                    Invoke(nameof(JumpToNextScene), glowDuration + 0.2f);
                }
            }
            else
            {
                // 顺序错误，可添加错误反馈（如红色闪烁）
                Debug.Log("顺序错误！");
            }
        }
    }

    // 发光效果
    void Glow()
    {
        if (rend != null && glowMaterial != null)
        {
            rend.material = glowMaterial;
            isGlowing = true;
            Invoke(nameof(ResetMaterial), glowDuration); // 恢复原始材质
        }
    }

    // 恢复原始材质
    void ResetMaterial()
    {
        if (rend != null && originalMaterial != null)
        {
            rend.material = originalMaterial;
            isGlowing = false;
        }
    }

    // 跳转到下一关
    void JumpToNextScene()
    {
        // 替换为你的下一关场景名
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
    }
}