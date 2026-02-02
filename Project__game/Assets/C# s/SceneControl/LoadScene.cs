using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadManger : MonoBehaviour
{
    [SerializeField] GameObject loadScreen;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] GameObject ChoseInterface;
    [SerializeField] GameObject StartInterface;
    [SerializeField] int loadIndex;

    public void LoadNextLevel()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        loadScreen.SetActive(true);
        ChoseInterface.SetActive(false);
        StartInterface.SetActive(false);

        //加载下一个场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadIndex);
        //"Async"是异步的意思   LoadScene会直接跳转到下一个场景，Async对应使用，上面是获取当前场景的builedIndex

        operation.allowSceneActivation = false;
        //是否加载新的场景
        //如果需要自动加载则不用写

        while (!operation.isDone)  //直接判断AsyncOperation里面的方法的isDone来判断加载是否结束，从而决定是否结束加载
        {
            slider.value = operation.progress;
            //使进度条等于加载进度

            textMeshPro.text = operation.progress * 100 + "%";

            if (operation.progress >= 0.9f)  //
            {
                slider.value = 1;

                textMeshPro.text = "Press Any Key To Continue";

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
            //停止携程
        }
    }
}