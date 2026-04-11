using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 字幕管理器 - 单例类
/// 负责显示和隐藏屏幕字幕
/// </summary>
public class SubtitleManager : MonoBehaviour
{
    // 单例实例
    public static SubtitleManager Instance { get; private set; }

    [Header("UI元素引用")]
    [SerializeField] private Canvas subtitleCanvas;
    [SerializeField] private Text subtitleText;
    [SerializeField] private Image backgroundImage;

    [Header("默认设置")]
    [Tooltip("字幕位置偏移 (屏幕坐标)")]
    public Vector2 screenPosition = new Vector2(0, 0.1f); // 底部10%位置

    [Tooltip("字幕宽度占屏幕宽度的比例")]
    [Range(0.2f, 0.8f)]
    public float widthRatio = 0.7f;

    [Tooltip("默认字体大小")]
    public int defaultFontSize = 24;

    [Tooltip("默认文字颜色")]
    public Color defaultTextColor = Color.white;

    [Tooltip("默认背景颜色")]
    public Color defaultBgColor = new Color(0, 0, 0, 0.7f);

    [Tooltip("默认显示时间")]
    public float defaultDisplayTime = 3f;

    [Header("淡入淡出设置")]
    [Tooltip("默认淡入时间")]
    public float defaultFadeInTime = 0.5f;

    [Tooltip("默认淡出时间")]
    public float defaultFadeOutTime = 0.7f;

    [Header("调试")]
    [Tooltip("启用调试日志")]
    public bool enableDebugLog = true;

    private Coroutine currentCoroutine;
    private RectTransform textRectTransform;
    private RectTransform bgRectTransform;
    private CanvasGroup textCanvasGroup;
    private CanvasGroup bgCanvasGroup;

    private void Awake()
    {
        // 单例模式初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 自动初始化UI
        if (subtitleCanvas == null)
        {
            InitializeUI();
        }
        else
        {
            SetupUIComponents();
        }

        if (enableDebugLog)
        {
            Debug.Log("SubtitleManager 初始化完成");
        }
    }

    private void InitializeUI()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("SubtitleCanvas");
        subtitleCanvas = canvasObj.AddComponent<Canvas>();
        subtitleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Canvas排序
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建背景
        GameObject bgObj = new GameObject("SubtitleBackground");
        bgObj.transform.SetParent(canvasObj.transform, false);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = defaultBgColor;

        // 创建文字
        GameObject textObj = new GameObject("SubtitleText");
        textObj.transform.SetParent(canvasObj.transform, false);
        subtitleText = textObj.AddComponent<Text>();
        subtitleText.text = "";
        subtitleText.color = defaultTextColor;
        subtitleText.fontSize = defaultFontSize;
        subtitleText.alignment = TextAnchor.MiddleCenter;

        // 设置字体
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont != null)
        {
            subtitleText.font = defaultFont;
        }

        // 设置CanvasGroup
        textCanvasGroup = textObj.AddComponent<CanvasGroup>();
        bgCanvasGroup = bgObj.AddComponent<CanvasGroup>();

        // 获取RectTransform
        textRectTransform = subtitleText.GetComponent<RectTransform>();
        bgRectTransform = backgroundImage.GetComponent<RectTransform>();

        // 初始隐藏
        textCanvasGroup.alpha = 0;
        bgCanvasGroup.alpha = 0;

        DontDestroyOnLoad(canvasObj);
    }

    private void SetupUIComponents()
    {
        if (subtitleText == null)
        {
            Debug.LogError("请为SubtitleManager指定SubtitleText");
            return;
        }

        // 确保有CanvasGroup
        textCanvasGroup = subtitleText.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
        {
            textCanvasGroup = subtitleText.gameObject.AddComponent<CanvasGroup>();
        }

        if (backgroundImage != null)
        {
            bgCanvasGroup = backgroundImage.GetComponent<CanvasGroup>();
            if (bgCanvasGroup == null)
            {
                bgCanvasGroup = backgroundImage.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // 获取RectTransform
        textRectTransform = subtitleText.GetComponent<RectTransform>();
        if (backgroundImage != null)
        {
            bgRectTransform = backgroundImage.GetComponent<RectTransform>();
        }

        // 初始隐藏
        textCanvasGroup.alpha = 0;
        if (bgCanvasGroup != null)
        {
            bgCanvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 显示字幕
    /// </summary>
    /// <param name="message">字幕内容</param>
    /// <param name="displayTime">显示时间</param>
    /// <param name="textColor">文字颜色</param>
    /// <param name="bgColor">背景颜色</param>
    /// <param name="fontSize">字体大小</param>
    public void ShowSubtitle(string message, float displayTime = -1,
                           Color? textColor = null, Color? bgColor = null,
                           int? fontSize = null)
    {
        if (string.IsNullOrEmpty(message) || subtitleText == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("无法显示字幕：消息为空或Text组件未设置");
            }
            return;
        }

        // 停止之前的协程
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 更新字幕内容
        subtitleText.text = message;

        // 应用样式
        subtitleText.color = textColor ?? defaultTextColor;
        subtitleText.fontSize = fontSize ?? defaultFontSize;

        if (backgroundImage != null)
        {
            backgroundImage.color = bgColor ?? defaultBgColor;
        }

        // 计算并设置位置大小
        UpdateUIPosition();

        // 开始显示序列
        currentCoroutine = StartCoroutine(ShowSequence(displayTime));
    }

    private IEnumerator ShowSequence(float displayTime)
    {
        float actualDisplayTime = displayTime > 0 ? displayTime : defaultDisplayTime;

        if (enableDebugLog)
        {
            Debug.Log($"显示字幕: \"{subtitleText.text}\" ({actualDisplayTime}秒)");
        }

        // 淡入
        yield return FadeCanvasGroup(textCanvasGroup, 0, 1, defaultFadeInTime);
        if (bgCanvasGroup != null)
        {
            yield return FadeCanvasGroup(bgCanvasGroup, 0, 1, defaultFadeInTime);
        }

        // 保持显示
        yield return new WaitForSeconds(actualDisplayTime);

        // 淡出
        yield return FadeCanvasGroup(textCanvasGroup, 1, 0, defaultFadeOutTime);
        if (bgCanvasGroup != null)
        {
            yield return FadeCanvasGroup(bgCanvasGroup, 1, 0, defaultFadeOutTime);
        }

        currentCoroutine = null;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float fromAlpha, float toAlpha, float duration)
    {
        if (group == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            yield return null;
        }
        group.alpha = toAlpha;
    }

    /// <summary>
    /// 立即隐藏字幕
    /// </summary>
    public void HideSubtitleImmediate()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0;
        }

        if (bgCanvasGroup != null)
        {
            bgCanvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 立即显示字幕（无淡入）
    /// </summary>
    public void ShowSubtitleImmediate(string message, Color? textColor = null,
                                    Color? bgColor = null, int? fontSize = null)
    {
        if (string.IsNullOrEmpty(message) || subtitleText == null) return;

        subtitleText.text = message;
        subtitleText.color = textColor ?? defaultTextColor;
        subtitleText.fontSize = fontSize ?? defaultFontSize;

        if (backgroundImage != null)
        {
            backgroundImage.color = bgColor ?? defaultBgColor;
        }

        UpdateUIPosition();

        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 1;
        }

        if (bgCanvasGroup != null)
        {
            bgCanvasGroup.alpha = 1;
        }
    }

    private void UpdateUIPosition()
    {
        if (textRectTransform == null) return;

        // 计算屏幕中心偏移
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float width = screenWidth * widthRatio;
        float height = 100f; // 固定高度

        // 设置文字位置
        textRectTransform.sizeDelta = new Vector2(width, height);
        textRectTransform.anchoredPosition = new Vector2(
            screenPosition.x * screenWidth,
            screenPosition.y * screenHeight
        );

        // 设置背景位置
        if (bgRectTransform != null)
        {
            // 背景比文字大一些
            bgRectTransform.sizeDelta = new Vector2(width + 40, height + 20);
            bgRectTransform.anchoredPosition = textRectTransform.anchoredPosition;
        }
    }

    /// <summary>
    /// 测试方法：显示示例字幕
    /// </summary>
    [ContextMenu("测试显示字幕")]
    public void TestSubtitle()
    {
        ShowSubtitle("这是一个测试字幕，将持续3秒", 3f, Color.yellow, new Color(0, 0, 0, 0.8f), 28);
    }

    [ContextMenu("测试隐藏字幕")]
    public void TestHideSubtitle()
    {
        HideSubtitleImmediate();
    }
}
