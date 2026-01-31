using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayChangeSound : MonoBehaviour
{
    public AudioClip changeSound; // 拖拽场景切换音效到这里
    private AudioSource audioSource;

    private void Start()
    {
        // 给物体添加AudioSource组件（现在没音乐
        audioSource = gameObject.AddComponent<AudioSource>();
        // 挂载到多播委托
        SceneChangeDelegate.OnSceneChange += PlaySound;
    }

    // 播放切换音效
    private void PlaySound()
    {
        audioSource.PlayOneShot(changeSound);
        Debug.Log("多播委托执行：播放场景切换音效");
    }

    private void OnDestroy()
    {
        SceneChangeDelegate.OnSceneChange -= PlaySound;
    }
}