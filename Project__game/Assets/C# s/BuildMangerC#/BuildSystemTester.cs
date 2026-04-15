// 测试脚本：检查系统状态
using UnityEngine;
using System.Collections;

public class BuildSystemTester : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestBuildingSequence());
    }

    IEnumerator TestBuildingSequence()
    {
        Debug.Log("=== 建筑系统测试开始 ===");

        // 1. 检查管理器
        BuildSystemManager manager = FindObjectOfType<BuildSystemManager>();
        if (manager == null)
        {
            Debug.LogError("未找到BuildSystemManager!");
            yield break;
        }

        Debug.Log("找到BuildSystemManager");

        // 2. 检查吸附点
        SnapPoint[] snapPoints = FindObjectsOfType<SnapPoint>();
        Debug.Log($"找到{snapPoints.Length}个吸附点");

        // 3. 检查预制体
        if (manager.buildItemPrefabs.Count == 0)
        {
            Debug.LogError("建造队列为空!");
            yield break;
        }

        Debug.Log($"建造队列中有{manager.buildItemPrefabs.Count}个物体");

        // 4. 模拟按键
        Debug.Log("请按F键开始建造...");

        yield return new WaitForSeconds(1);
        Debug.Log("=== 测试完成 ===");
    }
}
//AI辅助生成:Hunyuan