using UnityEngine;
using System.Collections;

public class DragAndSnap : MonoBehaviour
{
    [Header("旋转设置")]
    public Vector3 targetEulerAngles = new Vector3(0, 90, 0); // 目标角度
    public float snapAngleThreshold = 15f; // 进入吸附的触发角度差

    [Header("拖拽设置")]
    public float rotationSpeed = 200f; // 拖拽时的旋转速度
    public float smoothSnapTime = 0.3f; // 吸附动画的平滑时间

    [Header("状态")]
    public bool isSnapped = false; // 当前是否已吸附对齐
    public bool isDragging = false; // 是否正在被拖拽

    private Vector3 dragStartPosition;
    private float dragStartAngle;
    private float currentVelocity; // 用于平滑阻尼
    private Quaternion targetRotation; // 目标旋转

    void Update()
    {
        // 如果不是手动拖拽且未吸附，检查是否需要自动吸附
        if (!isDragging && !isSnapped)
        {
            CheckForAutoSnap();
        }

        // 实时检测是否已正确对齐（用于过关判定）
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(targetEulerAngles)) < 5f)
        {
            if (!isSnapped)
            {
                OnSnapComplete();
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        isSnapped = false;
        dragStartPosition = Input.mousePosition;
        dragStartAngle = transform.eulerAngles.y;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        // 计算鼠标移动量并转换为旋转
        Vector3 currentPos = Input.mousePosition;
        float deltaX = (currentPos.x - dragStartPosition.x) * rotationSpeed / Screen.width;
        float newAngle = dragStartAngle + deltaX;

        // 应用旋转（只绕Y轴旋转）
        transform.rotation = Quaternion.Euler(0, newAngle, 0);

        // 拖拽时实时检查是否接近目标角度
        CheckProximityFeedback();
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 松开鼠标时，检查是否需要自动吸附
        if (ShouldSnap())
        {
            StartCoroutine(SmoothSnapToTarget());
        }
    }

    // 检查是否应该触发吸附
    bool ShouldSnap()
    {
        float angleDiff = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetEulerAngles));
        return angleDiff <= snapAngleThreshold;
    }

    // 检查并处理自动吸附
    void CheckForAutoSnap()
    {
        if (ShouldSnap())
        {
            StartCoroutine(SmoothSnapToTarget());
        }
    }

    // 平滑吸附到目标角度的协程
    IEnumerator SmoothSnapToTarget()
    {
        isSnapped = true;
        targetRotation = Quaternion.Euler(targetEulerAngles);

        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < smoothSnapTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / smoothSnapTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        // 吸附完成后的回调
        OnSnapComplete();
    }

    // 接近目标时的视觉反馈（例如：物体发光、震动）
    void CheckProximityFeedback()
    {
        float angleDiff = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetEulerAngles));

        if (angleDiff < snapAngleThreshold)
        {
            // 接近时物体高亮
            GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow * (1 - angleDiff / snapAngleThreshold));
        }
        else
        {
            // 恢复原色
            GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }

    // 吸附完成时的处理
    void OnSnapComplete()
    {
        Debug.Log($"{gameObject.name} 已正确对齐！");
        isSnapped = true;

        // 触发过关事件
        SendMessageUpwards("OnPuzzleSolved", SendMessageOptions.DontRequireReceiver);

        // 播放音效
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null) audio.Play();

        // 视觉确认：绿色高亮
        StartCoroutine(HighlightEffect());
    }

    // 高亮特效
    IEnumerator HighlightEffect()
    {
        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;
        mat.color = Color.green;

        yield return new WaitForSeconds(0.5f);

        mat.color = originalColor;
    }
}
