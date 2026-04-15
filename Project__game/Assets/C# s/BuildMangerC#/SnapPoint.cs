using UnityEngine;

[System.Serializable]
public class SnapPointSlot
{
    [Tooltip("已有建筑：从 Hierarchy 拖入，运行后会在该物体中心生成吸附点；与建造队列同序号对应")]
    public GameObject buildingTarget;
    [Tooltip("吸附类型，与要吸附到此点的建筑的 BuildableItem.snapType 一致")]
    public string snapType = "Default";
    [Tooltip("相对物体中心的偏移，(0,0,0)即物体自身中心")]
    public Vector3 localOffset = Vector3.zero;
}

public class SnapPoint : MonoBehaviour
{
    public string snapType = "Default";
    public bool isOccupied = false;
    public bool showGizmo = true;
    public Color gizmoColor = Color.green;
    public float gizmoSize = 0.3f;

    void Start()
    {
        if (BuildSystemManager.Instance != null)
            BuildSystemManager.Instance.RegisterSnapPoint(this);
    }

    void OnDestroy()
    {
        if (BuildSystemManager.Instance != null)
            BuildSystemManager.Instance.UnregisterSnapPoint(this);
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = isOccupied ? Color.red : gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * gizmoSize * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * gizmoSize * 2);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoSize * 1.5f);
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = gizmoColor;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
            snapType + (isOccupied ? "\n(占用)" : "\n(空闲)"), style);
#endif
    }
}
//AI辅助生成:Hunyuan