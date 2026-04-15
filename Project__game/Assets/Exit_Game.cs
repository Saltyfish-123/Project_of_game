using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void ExitGame()
    {
        // 退出游戏
        Application.Quit();

        // 在Unity编辑器中停止运行（仅用于测试）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}