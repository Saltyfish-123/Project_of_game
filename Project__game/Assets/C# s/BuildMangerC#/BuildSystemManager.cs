using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BuildState
{
    None,
    Moving,
    Rotating,
    Snapped,
    Placed
}

public class BuildSystemManager : MonoBehaviour
{
    public static BuildSystemManager Instance;

    [Header("建造设置")]
    public BuildState currentBuildState = BuildState.None;
    public float snapDistance = 1.5f;
    public LayerMask snapPointLayer;
    [Tooltip("放置时射线忽略的层级，可勾选 Player 避免建造物贴脸推人")]
    public LayerMask placementIgnoreLayers = 0;
    public Material validMaterial;
    public Material invalidMaterial;
    public Material snappedMaterial;

    [Header("当前建造物体")]
    public GameObject currentBuildingItem;
    public BuildableItem currentBuildableItem;

    [Header("建造队列")]
    [Tooltip("将预制体拖入此列表，预制体需挂 BuildableItem")]
    public List<GameObject> buildItemPrefabs = new List<GameObject>();
    [Header("吸附点队列（与建造队列序号一一对应）")]
    [Tooltip("从 Hierarchy 把已有建筑拖入对应序号；运行后会在该建筑身上生成吸附点，建造队列中同序号的建筑可吸附到此")]
    public List<SnapPointSlot> snapPointsForBuildQueue = new List<SnapPointSlot>();
    private Queue<GameObject> buildItemQueue = new Queue<GameObject>();
    private int placedCount;

    [Header("建造完成")]
    public UnityEvent endEvent;

    [Header("输入设置")]
    public KeyCode buildKey = KeyCode.F;
    public KeyCode rotateKey = KeyCode.R;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode unsnapKey = KeyCode.LeftShift;

    [Header("视角前方放置")]
    [Tooltip("物体在摄像机正前方的距离，滚轮可调")]
    public float placementDistance = 5f;
    public float placementDistanceMin = 1f;
    public float placementDistanceMax = 50f;
    public float scrollStep = 1f;

    private Camera mainCamera;
    private List<SnapPoint> allSnapPoints = new List<SnapPoint>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;

        foreach (GameObject prefab in buildItemPrefabs)
            buildItemQueue.Enqueue(prefab);

        CreateSnapPointsOnListedBuildings();
    }

    void CreateSnapPointsOnListedBuildings()
    {
        for (int i = 0; i < snapPointsForBuildQueue.Count; i++)
        {
            SnapPointSlot slot = snapPointsForBuildQueue[i];
            if (slot.buildingTarget == null || string.IsNullOrEmpty(slot.snapType)) continue;
            CreateCenterSnapPointOn(slot.buildingTarget, slot.snapType, slot.localOffset);
        }
    }

    void CreateCenterSnapPointOn(GameObject building, string snapType, Vector3 localOffset)
    {
        if (building == null) return;
        Transform parent = building.transform;
        GameObject snapGo = new GameObject("CenterSnapPoint");
        snapGo.transform.SetParent(parent, false);
        snapGo.transform.localPosition = localOffset;
        snapGo.transform.localRotation = Quaternion.identity;
        snapGo.transform.localScale = Vector3.one;
        SnapPoint sp = snapGo.AddComponent<SnapPoint>();
        sp.snapType = snapType;
        sp.isOccupied = false;
    }

    void Update()
    {
        HandleInput();
        UpdateCurrentItemState();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(buildKey) && currentBuildState == BuildState.None)
        {
            StartBuildingMode();
        }

        if (Input.GetKeyDown(cancelKey) && currentBuildState != BuildState.None)
        {
            CancelCurrentBuild();
        }

        if (currentBuildableItem != null)
        {
            currentBuildableItem.HandleInput(currentBuildState);
        }

        if (currentBuildState == BuildState.Moving && currentBuildingItem != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) placementDistance = Mathf.Clamp(placementDistance - scrollStep, placementDistanceMin, placementDistanceMax);
            if (scroll < 0f) placementDistance = Mathf.Clamp(placementDistance + scrollStep, placementDistanceMin, placementDistanceMax);
        }
    }

    public Vector3 GetViewCenterWorldPosition(float distance)
    {
        if (mainCamera == null) mainCamera = Camera.main;
        return mainCamera.transform.position + mainCamera.transform.forward * Mathf.Clamp(distance, placementDistanceMin, placementDistanceMax);
    }

    void StartBuildingMode()
    {
        if (buildItemQueue.Count == 0)
        {
            Debug.Log("建造队列为空");
            return;
        }

        GameObject nextPrefab = buildItemQueue.Dequeue();

        // 在摄像机正前方中间、空中生成，距离由 placementDistance 控制（滚轮调节）
        Vector3 spawnPosition = GetViewCenterWorldPosition(placementDistance);

        currentBuildingItem = Instantiate(nextPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"物体生成在：{spawnPosition}");

        currentBuildableItem = currentBuildingItem.GetComponent<BuildableItem>();

        if (currentBuildableItem == null)
        {
            currentBuildableItem = currentBuildingItem.AddComponent<BuildableItem>();
        }

        currentBuildableItem.Initialize(this);
        currentBuildState = BuildState.Moving;

        Debug.Log("进入建造模式，当前状态: Moving");
    }
    void UpdateCurrentItemState()
    {
        if (currentBuildableItem == null) return;

        switch (currentBuildState)
        {
            case BuildState.Moving:
                currentBuildableItem.UpdateMoving();
                break;
            case BuildState.Rotating:
                currentBuildableItem.UpdateRotating();
                break;
            case BuildState.Snapped:
                currentBuildableItem.UpdateSnapped();
                break;
        }
    }

    public void RegisterSnapPoint(SnapPoint snapPoint)
    {
        if (!allSnapPoints.Contains(snapPoint))
        {
            allSnapPoints.Add(snapPoint);
        }
    }

    public void UnregisterSnapPoint(SnapPoint snapPoint)
    {
        allSnapPoints.Remove(snapPoint);
    }

    public SnapPoint FindNearestSnapPoint(Vector3 position, string snapType = "")
    {
        SnapPoint nearestSnap = null;
        float nearestDistance = float.MaxValue;

        foreach (SnapPoint snapPoint in allSnapPoints)
        {
            if (snapPoint.isOccupied) continue;

            if (!string.IsNullOrEmpty(snapType) && snapPoint.snapType != snapType)
                continue;

            float distance = Vector3.Distance(position, snapPoint.transform.position);

            if (distance < snapDistance && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSnap = snapPoint;
            }
        }

        return nearestSnap;
    }

    public void SwitchToState(BuildState newState)
    {
        if (currentBuildState == newState) return;

        currentBuildState = newState;
        Debug.Log("建造状态切换到: " + newState);
    }

    public void ConfirmPlacement()
    {
        if (currentBuildableItem == null) return;

        currentBuildableItem.PlaceItem();
        currentBuildState = BuildState.Placed;

        if (currentBuildableItem.snappedPoint != null)
            currentBuildableItem.snappedPoint.isOccupied = true;

        int placedIndex = placedCount;
        placedCount++;
        CreateCenterSnapPointForPlaced(placedIndex, currentBuildingItem);

        currentBuildableItem = null;
        currentBuildingItem = null;

        if (buildItemQueue.Count > 0)
            StartBuildingMode();
        else
        {
            currentBuildState = BuildState.None;
            placedCount = 0;
            Debug.Log("所有物体建造完成");
            UnLockCursor();
            endEvent?.Invoke();
        }
    }

    void CreateCenterSnapPointForPlaced(int buildIndex, GameObject placedObject)
    {
        if (placedObject == null) return;
        if (buildIndex < 0 || buildIndex >= snapPointsForBuildQueue.Count) return;

        SnapPointSlot slot = snapPointsForBuildQueue[buildIndex];
        if (string.IsNullOrEmpty(slot.snapType)) return;

        CreateCenterSnapPointOn(placedObject, slot.snapType, slot.localOffset);
    }

    void CancelCurrentBuild()
    {
        if (currentBuildableItem != null)
        {
            currentBuildableItem.CancelBuild();
        }

        if (currentBuildingItem != null)
        {
            Destroy(currentBuildingItem);
        }

        if (buildItemQueue.Count > 0)
        {
            buildItemQueue.Enqueue(buildItemPrefabs[0]);
        }

        currentBuildableItem = null;
        currentBuildingItem = null;
        currentBuildState = BuildState.None;

        Debug.Log("建造取消");
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 100f;

        // 若有正在移动的建造物，用 RaycastAll 并忽略它，避免射线打到预览物导致位置贴脸/推人
        if (currentBuildingItem != null)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit h in hits)
            {
                if (h.collider.transform.IsChildOf(currentBuildingItem.transform) || h.collider.gameObject == currentBuildingItem)
                    continue;
                if (placementIgnoreLayers != 0 && ((1 << h.collider.gameObject.layer) & placementIgnoreLayers) != 0)
                    continue;
                return h.point;
            }
        }
        else
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit h in hits)
            {
                if (placementIgnoreLayers != 0 && ((1 << h.collider.gameObject.layer) & placementIgnoreLayers) != 0)
                    continue;
                return h.point;
            }
        }

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }

    public void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}