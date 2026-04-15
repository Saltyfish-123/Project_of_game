using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   
using System.Collections;  //协程接口

//多播委托  
public delegate void SceneChangeAction();
//进度回调委托（给进度条传值，参数为0-1的加载进度）
public delegate void LoadProgressAction(float progress);

public class SceneChangeDelegate : MonoBehaviour
{
    //多播委托：关闭面板 保存数据
    public static SceneChangeAction OnSceneChange;  //调用一次性执行所有挂载的方法
    //进度回调：进度条更新
    public static LoadProgressAction OnLoadProgress;//传书进度

    // 单例：指挥中心：其他脚本可以通过SceneChangeDelegate Instance直接调用他的方法
    public static SceneChangeDelegate Instance;
    //拖拽场景中的进度条到这(加载界面中的进度条，美工设计
    public Slider loadSlider;
    //拖拽进度百分比文本到这
    public Text progressText;

    private void Awake()
    {
        // 初始化单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 初始化进度条：隐藏（加载的时候再显示）
        if (loadSlider != null) loadSlider.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
    }


    //异步加载核心（协程），参数：目标场景名
    public IEnumerator TriggerSceneChangeAsync(string sceneName)
    {
        //1.执行多播委托（关闭面板/保存数据/播放音效等
        OnSceneChange?.Invoke();

        //2.显示进度条，初始化进度为0
        if (loadSlider != null)
        {
            loadSlider.gameObject.SetActive(true);
            loadSlider.value = 0;
        }
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "加载中：0%";
        }

        //3.开始异步加载场景
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false; // 禁止自动激活场景

        //4.循环更新加载进度
        while (asyncOp.progress < 0.9f) //0.9=加载完成，1.0=激活场景
        {
            float progress = asyncOp.progress; // 0-0.9的原始进度
            float showProgress = Mathf.Lerp(0, 1, progress / 0.9f); // 转换为0-1的显示进度

            // 调用进度回调：给外部进度条传值（兼容多进度条）
            OnLoadProgress?.Invoke(showProgress);
            // 直接更新内部绑定的进度条
            if (loadSlider != null) loadSlider.value = showProgress;
            if (progressText != null) progressText.text = $"加载中：{(int)(showProgress * 100)}%";

            yield return null; // 每帧更新
        }

        //5.加载完成
        OnLoadProgress?.Invoke(1);
        if (loadSlider != null) loadSlider.value = 1;
        if (progressText != null) progressText.text = "加载完成：100%";
        yield return new WaitForSeconds(0.5f); // 停留0.5秒，让玩家看到100%

        //6.激活新场景，完成切换
        asyncOp.allowSceneActivation = true;
    }

    // 移除委托方法
    public static void RemoveSceneChangeMethod(SceneChangeAction method)
    {
        OnSceneChange -= method;
    }
    public static void RemoveLoadProgressMethod(LoadProgressAction method)
    {
        OnLoadProgress -= method;
    }
}