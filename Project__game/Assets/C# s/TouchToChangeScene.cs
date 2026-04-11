using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToChangeScene : MonoBehaviour
{
    [Tooltip("目标场景名（必须已在Build Settings中添加）")]
    public string targetSceneName = "Scene2";

    [Tooltip("触发后延迟几秒跳转（默认0.5）")]
    public float delayBeforeJump = 0.5f;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("触碰到桥，3秒后跳往:Scene5");
            Invoke(nameof(DoSceneJump), delayBeforeJump);
        }
    }

    private void DoSceneJump()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
            SceneManager.LoadScene(targetSceneName);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.15f, 0.85f, 1f, 0.32f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
#endif
}