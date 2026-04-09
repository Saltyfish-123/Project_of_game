using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public class PuzzlePiece
    {
        public GameObject pieceObject;
        public Vector3 correctRotation;
        [HideInInspector] public bool isCorrect = false;
    }

    [Header("谜题设置")]
    public List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();
    public float toleranceAngle = 5f; // 过关的容错角度

    [Header("事件")]
    public UnityEvent onPuzzleSolved; // 过关时触发的事件

    private bool isSolved = false;

    void Update()
    {
        if (isSolved) return;

        CheckAllPieces();
    }

    // 检查所有碎片是否都在正确位置
    void CheckAllPieces()
    {
        foreach (PuzzlePiece piece in puzzlePieces)
        {
            float angleDiff = Quaternion.Angle(
                piece.pieceObject.transform.rotation,
                Quaternion.Euler(piece.correctRotation)
            );

            piece.isCorrect = angleDiff <= toleranceAngle;

            if (!piece.isCorrect) return; // 有一个不对就退出
        }

        // 全部正确
        OnPuzzleSolved();
    }

    // 单个物体对齐时的回调（由 DragAndSnap 调用）
    public void OnPuzzleSolved()
    {
        if (isSolved) return;

        isSolved = true;
        Debug.Log("谜题解决！过关！");

        onPuzzleSolved.Invoke(); // 触发事件（开门、播放动画等）

        // 示例：3秒后加载下一关
        // Invoke("LoadNextLevel", 3f);
    }

    void LoadNextLevel()
    {
        // 你的关卡加载逻辑
    }
}
