using Cinemachine.Examples;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CGTrigger : MonoBehaviour
{
    public Animator cgAnimator;
    public string cgTriggerName = "BridgeCG";

    [Header("事件")]
    public UnityEvent onTriggerEntered = new UnityEvent();
    public UnityEvent onAnimationStart = new UnityEvent();
    public UnityEvent onBeforeSceneLoad = new UnityEvent();
    public UnityEvent onAnimationComplete = new UnityEvent();

    [Header("场景加载")]
    public string targetSceneName = "BuildingScene";
    public float sceneLoadDelay = 2f;
    private bool isTriggered = false;

    void Start()
    {
            if (cgAnimator == null) Debug.LogWarning("无Animator");
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
           Debug.Log("CG触发");

            isTriggered = true;
            StartCoroutine(PlayCGSequence());
        }
    }

    IEnumerator PlayCGSequence()
    {
        onTriggerEntered?.Invoke();

        if (cgAnimator != null && !string.IsNullOrEmpty(cgTriggerName))
        {
            cgAnimator.SetTrigger(cgTriggerName);
        }

        onAnimationStart?.Invoke();

        onBeforeSceneLoad?.Invoke();

        yield return new WaitForSeconds(sceneLoadDelay);

        onAnimationComplete?.Invoke();

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("目标场景名称为空!");

            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if ( GetComponent<Collider>() != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
        }
    }
#endif
}
