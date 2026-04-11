using UnityEngine;

public class StepManager : MonoBehaviour
{
    // 单例实例
    public static StepManager Instance;

    // 正确的踩字顺序（对应石台字的标签）
    private string[] correctOrder = { "定", "基", "架", "梁", "铺", "板", "成", "桥" };
    // 当前踩到第几步（0~7，对应顺序索引）
    private int currentStep = 0;

    void Awake()
    {
        // 单例模式：确保场景只有一个实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 验证踩的字是否符合顺序
    public bool CheckStep(string character)
    {
        if (currentStep >= correctOrder.Length) return false; // 已走完所有步骤
        if (character == correctOrder[currentStep])
        {
            currentStep++; // 步骤+1
            // 检查是否走完所有步骤
            if (currentStep >= correctOrder.Length)
            {
                return true; // 顺序正确，触发跳转
            }
            return true; // 顺序正确，继续下一步
        }
        else
        {
            // 顺序错误，重置进度
            ResetSteps();
            return false;
        }
    }

    // 重置进度（可扩展：比如播放错误音效、视觉反馈）
    public void ResetSteps()
    {
        currentStep = 0;
        Debug.Log("顺序错误，已重置！");
    }
}