using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家位置触发字幕脚本
/// 当玩家走到触发器区域时，在屏幕指定位置显示字幕
/// </summary>
public class SubtitleTrigger : MonoBehaviour
{
    [Header("字幕内容")]
    [Tooltip("要显示的文字内容，支持多行")]
    [TextArea(2, 5)]
    public string subtitleMessage = "这里似乎有什么机关...";

    [Header("显示设置")]
    [Tooltip("字幕显示持续时间（秒）")]
    public float displayTime = 3f;
    [Tooltip("淡入时间（秒）")]
    public float fadeInDuration = 0.5f;
    [Tooltip("淡出时间（秒）")]
    public float fadeOutDuration = 0.7f;

    [Header("触发设置")]
    [Tooltip("是否只触发一次")]
    public bool oneTimeOnly = true;
    [Tooltip("触发半径（用于可视化）")]
    public float triggerRadius = 3f;

    [Header("UI样式")]
    [Tooltip("字体大小（默认24）")]
    public int fontSize = 24;
    [Tooltip("文字颜色（默认白色）")]
    public Color textColor = Color.white;
    [Tooltip("背景颜色（默认半透明黑）")]
    public Color bgColor = new Color(0, 0, 0, 0.7f);

    private bool hasTriggered = false;

    void Start()
    {
        // 确保有碰撞器
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
            SphereCollider sphereCol = (SphereCollider)col;
            sphereCol.radius = triggerRadius;
        }

        // 设置为触发器
        col.isTrigger = true;
    }

    /// <summary>
    /// 玩家进入触发器区域
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (hasTriggered && oneTimeOnly) return;
        if (!other.CompareTag("Player")) return;

        TriggerSubtitle();
    }

    /// <summary>
    /// 触发字幕显示
    /// </summary>
    public void TriggerSubtitle()
    {
        hasTriggered = true;
        SubtitleManager.Instance?.ShowSubtitle(subtitleMessage, displayTime,textColor, bgColor, fontSize);
    }

    /// <summary>
    /// 强制显示字幕（可从其他脚本调用）
    /// </summary>
    public void ForceShowSubtitle()
    {
        SubtitleManager.Instance?.ShowSubtitle(subtitleMessage, displayTime,
                                             textColor, bgColor, fontSize);
    }


    /// <summary>
    /// 启用触发器
    /// </summary>
    public void EnableTrigger()
    {
        enabled = true;
    }

    /// <summary>
    /// 禁用触发器
    /// </summary>
    public void DisableTrigger()
    {
        enabled = false;
    }
}


