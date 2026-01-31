using UnityEngine;
using UnityEngine.UI;

public class UIPanelClose : MonoBehaviour
{
    public Button changeSceneBtn; // 拖场景切换按钮到这里
    private void Start()
    {
        // 给按钮绑定点击事件：点击后触发切换到Scene1
        changeSceneBtn.onClick.AddListener(() =>//监听
        {
            StartCoroutine(SceneChangeDelegate.Instance.TriggerSceneChangeAsync("Scene1"));
        });

        // 将关闭面板的方法挂载到多播委托上
        SceneChangeDelegate.OnSceneChange += ClosePanel;
    }

    //关闭当前面板
    private void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log("多播委托执行：关闭开始面板");
    }

    // 场景销毁时移除委托
    private void OnDestroy()
    {
        SceneChangeDelegate.OnSceneChange -= ClosePanel;
    }
}