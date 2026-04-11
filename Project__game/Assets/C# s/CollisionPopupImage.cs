using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollisionPopupImage : MonoBehaviour
{
    [Header("弹出图片")]
    public Image popupImage;           // UI中的Image组件
    public float showDuration = 3f;    // 显示时长（秒）
    public float fadeInTime = 0.3f;    // 淡入时间
    public float fadeOutTime = 0.5f;   // 淡出时间

    private CanvasGroup imgCanvasGroup;
    private bool isShowing;

    void Start()
    {
        // 确保有Image并初始隐藏
        if (popupImage != null)
        {
            imgCanvasGroup = popupImage.GetComponent<CanvasGroup>();
            if (imgCanvasGroup == null)
                imgCanvasGroup = popupImage.gameObject.AddComponent<CanvasGroup>();

            popupImage.gameObject.SetActive(false);
            imgCanvasGroup.alpha = 0;
        }

        // 确保自己是触发器
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && popupImage != null)
        {
            // 每次碰撞都重新开始显示流程
            StopAllCoroutines();
            StartCoroutine(ShowAndHide());
        }
    }

    IEnumerator ShowAndHide()
    {
        // 激活并淡入
        popupImage.gameObject.SetActive(true);
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            imgCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            yield return null;
        }
        imgCanvasGroup.alpha = 1;

        // 保持显示
        yield return new WaitForSeconds(showDuration);

        // 淡出
        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            imgCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            yield return null;
        }
        imgCanvasGroup.alpha = 0;
        popupImage.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.92f, 0.16f, 0.28f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
#endif
}