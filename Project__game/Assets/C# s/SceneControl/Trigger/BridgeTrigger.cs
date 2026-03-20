using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;


public class BridgeTrigger : MonoBehaviour
{

    private Color addColor = new Color(0, 0, 0, 0.01f);

    [Header("事件")]
    [SerializeField] private UnityEvent onTriggerEnteredEvent;
    [SerializeField] private UnityEvent onAnimationStartEvent;
    [SerializeField] private UnityEvent onBeforeSceneLoadEvent;
    [SerializeField] private UnityEvent onAnimationCompleteEvent;

    [Header("触发设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float triggerCooldown = 5f;// 防止重复触发的冷却时间

    [Header("场景切换")]
    [SerializeField] private string targetSceneName = "Scene2";// 目标场景名称
    [SerializeField] private float sceneSwitchDelay = 2f;// 场景切换前的延迟时间

    [Header("组件引用")]
    public MonoBehaviour CharacterMovemet;
    [SerializeField] private Animator bridgeAnimator;// 桥动画控制器
    [SerializeField] private Animation bridgeAnimation;

    [SerializeField] private CanvasGroup fadeCanvasGroup;// 用于渐黑效果的CanvasGroup
    [SerializeField] private Image BlackImage;
    [SerializeField] private float fadeDuration = 1.5f;// 渐黑持续时间

    [SerializeField] private bool isTriggered = false;// 是否已触发过
    [SerializeField] private float lastTriggerTime = -Mathf.Infinity;// 上次触发时间

    private void Awake()
    {
        onAnimationStartEvent.AddListener(PlayBridgeAnimation);
        onAnimationCompleteEvent.AddListener(StartFadeToBlack);
    }

    private void OnTriggerEnter(Collider other)// 触发器进入
    {
        if (isTriggered) return;// 已经触发过了，直接返回

        float timeSinceLastTrigger = Time.time - lastTriggerTime;// 计算距离上次触发的时间
        if (timeSinceLastTrigger < triggerCooldown) return;// 如果距离上次触发的时间不足冷却时间，直接返回

        if (other.CompareTag(playerTag))// 如果进入触发器的对象是玩家
        {
            isTriggered = true;// 标记为已触发
            lastTriggerTime = Time.time;// 更新上次触发时间
            StartCoroutine(ExecuteTriggerSequence());// 开始执行触发事件序列
        }
    }

    private IEnumerator ExecuteTriggerSequence()// 执行触发事件序列
    {

        Debug.Log("触发桥触发器");
        onTriggerEnteredEvent?.Invoke();
        yield return new WaitForSeconds(0.1f);// 等待触发事件完成后的一段时间，确保事件已经处理完毕

        //  动画开始事件
        onAnimationStartEvent?.Invoke();
        PlayBridgeAnimation();
        yield return new WaitForSeconds(0.5f);// 等待动画开始后的一段时间，确保动画已经开始播放

        //  场景加载前事件
        onBeforeSceneLoadEvent?.Invoke();
        yield return new WaitForSeconds(fadeDuration);// 等待渐黑动画完成

        //  动画完成事件
        onAnimationCompleteEvent?.Invoke();
        yield return new WaitForSeconds(sceneSwitchDelay);// 等待场景切换前的延迟时间

        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))// 如果目标场景名称不为空，加载目标场景
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("目标场景名称为空！");
        }
    }

    [ContextMenu("测试触发事件")]
    public void TestTriggerSequence()// 在Inspector中测试触发事件序列
    {
        if (isTriggered) return;// 已经触发过了，直接返回
        isTriggered = true;// 标记为已触发
        StartCoroutine(ExecuteTriggerSequence());// 开始执行触发事件序列
    }

    [ContextMenu("重置触发器")]
    public void ResetTrigger()// 重置触发器状态
    {
        isTriggered = false;
        if (fadeCanvasGroup != null)// 重置CanvasGroup状态
        {
            fadeCanvasGroup.alpha = 0f;// 透明
        }
    }

    // 提供给Inspector绑定的事件方法
    public void LockPlayer()
    {
        if (CharacterMovemet != null)// 锁定玩家控制器
        {
            CharacterMovemet.enabled = false;
            Debug.Log("玩家控制器已锁定");
        }
    }

    public void UnlockPlayer()
    {
        if (CharacterMovemet != null)// 解锁玩家控制器
        {
            CharacterMovemet.enabled = true;
            Debug.Log("玩家控制器已解锁");
        }
    }

    public void PlayBridgeAnimation()// 播放桥动画
    {
        if (bridgeAnimator != null)
        {
            bridgeAnimator.SetTrigger("BridgeCG");// 触发桥动画
            Debug.Log("播放桥动画");
        }
    }

    public void StartFadeToBlack()// 开始渐黑
    {
        if (BlackImage != null)
        {
            StartCoroutine(FadeToBlackCoroutine());// 启动渐黑协程
        }
    }

    private IEnumerator FadeToBlackCoroutine()// 渐黑协程
    {
        fadeCanvasGroup.interactable = true;// 允许交互，防止玩家在渐黑过程中操作
        fadeCanvasGroup.blocksRaycasts = true;// 阻止射线穿透，确保玩家无法点击其他UI元

        while (BlackImage.color.a < 1)
        {
            BlackImage.color += addColor;
            yield return null;
        }


        //float timer = 0f;// 初始化计时器
        //while (timer < fadeDuration)// 在fadeDuration时间内逐渐增加alpha值
        //{
        //    timer += Time.deltaTime;// 计算器++
        //    float alpha = 0f;
        //    fadeCanvasGroup.alpha = alpha;// 确保最终alpha值为1，完全覆盖屏幕
        //    alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);// 将alpha值应用到CanvasGroup
        //    yield return null;// 等待下一帧继续执行
        //}
        Debug.Log("场景已完全渐黑");
    }
}
