using Cinemachine.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGTrigger : MonoBehaviour
{
    // 你可以在Inspector中拖入你的CG动画控制器或
    public Animator cgAnimator;
    public string cgTriggerName = "BridgeCG";
    public float cgDuration = 5f; // 预估CG时长，用于自动恢复控制

    private CharacterMovement playerController;
    private bool isTriggered = false;

    void Start()
    {
        // 找到Player控制器
        playerController = FindObjectOfType<CharacterMovement>();
    }

    // 当Player进入触发区域时调用
    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return; // 防止重复触发

        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入触发区");
            isTriggered = true;
            StartCoroutine(BridgeCGSequence());
        }
    }

    // CG播放序列
    System.Collections.IEnumerator BridgeCGSequence()
    {
        // 1. 锁定Player输入
        if (playerController != null)
        {
        
            playerController.enabled = false;
        }

        // 2. 播放CG动画
        if (cgAnimator != null)
        {
            Debug.Log("开始播放动画");
            cgAnimator.SetTrigger(cgTriggerName);
        }

        // 3. 等待CG结束
        yield return new WaitForSeconds(cgDuration);

        // 4. 恢复Player控制
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 5. 销毁触发体，防止再次触发
        Destroy(gameObject);
    }
}