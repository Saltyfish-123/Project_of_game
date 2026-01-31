using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSave : MonoBehaviour
{
    public int playerCollect = 1; // 测试数据：玩家收集（道具
    public int playerLevel = 1;  // 测试数据：玩家等级（要升级吗？我也不知道

    private void Start()
    {
        // 挂载到多播委托
        SceneChangeDelegate.OnSceneChange += SavePlayerData;
    }

    // 保存数据
    private void SavePlayerData()
    {
        
        PlayerPrefs.SetInt("Collect", playerCollect);
        PlayerPrefs.SetInt("Level", playerLevel);
        PlayerPrefs.Save();
        Debug.Log("多播委托执行：保存玩家数据，收集：" + playerCollect);
    }

    private void OnDestroy()
    {
        SceneChangeDelegate.OnSceneChange -= SavePlayerData;
    }
}