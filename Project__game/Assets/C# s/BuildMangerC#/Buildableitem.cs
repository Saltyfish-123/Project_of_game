using UnityEngine;

public class BuildableItem : MonoBehaviour
{
    [Header("????????")]
    public string snapType = "Default";
    public float snapThreshold = 0.5f;
    [Tooltip("与吸附点重合的点的本地坐标（相对 pivot）。如 pivot 在中心且高 1，底面中心填 (0,-0.5,0) 可使底面与蓝色吸附面完全重合")]
    public Vector3 snapAnchorOffset = Vector3.zero;
    public bool useGridSnap = false;
    public float gridSize = 1f;

    [Header("旋转设置")]
    public float rotationSpeed = 90f;
    [Tooltip("上下旋转最大角度（度），避免翻转")]
    public float pitchClamp = 80f;
    public float snapRotationAngle = 45f;

    [Header("??")]
    public bool isPlaced = false;
    public SnapPoint snappedPoint;
    public bool isValidPlacement = true;

    private BuildSystemManager buildManager;
    private Renderer itemRenderer;
    private Material originalMaterial;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    void Start()
    {
        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
        {
            originalMaterial = itemRenderer.material;
        }

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();

        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    public void Initialize(BuildSystemManager manager)
    {
        buildManager = manager;
        isPlaced = false;

        // ?????? Initialize ????????????????? Initialize ?? Instantiate ???????????
        // ??? Start() ?????????itemRigidbody/itemCollider ??? null???????????????????????????
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rb in rbs)
            rb.isKinematic = true;

        Collider[] cols = GetComponentsInChildren<Collider>(true);
        foreach (Collider col in cols)
            col.isTrigger = true;
    }

    public void HandleInput(BuildState currentState)
    {
        if (isPlaced) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentState == BuildState.Moving)
            {
                buildManager.SwitchToState(BuildState.Rotating);
            }
            else if (currentState == BuildState.Rotating)
            {
                buildManager.SwitchToState(BuildState.Moving);
            }
        }

        if (Input.GetKeyDown(buildManager.unsnapKey) && currentState == BuildState.Snapped)
        {
            Unsnap();
            buildManager.SwitchToState(BuildState.Moving);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentState == BuildState.Snapped && isValidPlacement)
            {
                buildManager.ConfirmPlacement();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentState == BuildState.Rotating)
            {
                buildManager.SwitchToState(BuildState.Moving);
            }
        }
    }

    public void UpdateMoving()
    {
        float dist = buildManager.placementDistance;
        Vector3 targetPosition = buildManager.GetViewCenterWorldPosition(dist);

        if (useGridSnap)
            targetPosition = SnapToGrid(targetPosition);

        transform.position = targetPosition;
        snappedPoint = buildManager.FindNearestSnapPoint(targetPosition, snapType);

        if (snappedPoint != null && Vector3.Distance(targetPosition, snappedPoint.transform.position) < snapThreshold)
        {
            SnapToPoint(snappedPoint);
            buildManager.SwitchToState(BuildState.Snapped);
            UpdateVisualFeedback(true, true);
        }
        else
        {
            CheckPlacementValidity();
            UpdateVisualFeedback(isValidPlacement, false);
        }
    }

    public void UpdateRotating()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float yaw = transform.eulerAngles.y;
        float pitch = transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        CheckPlacementValidity();
        UpdateVisualFeedback(isValidPlacement, false);
    }

    public void UpdateSnapped()
    {
        float yaw = transform.eulerAngles.y;
        float pitch = transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        if (Input.GetKey(KeyCode.Q)) yaw -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) yaw += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Z)) pitch -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.C)) pitch += rotationSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (Input.GetKeyDown(KeyCode.T))
            SnapRotation();

        CheckPlacementValidity();
        UpdateVisualFeedback(isValidPlacement, true);
    }

    void SnapToPoint(SnapPoint snapPoint)
    {
        transform.rotation = snapPoint.transform.rotation;
        transform.position = snapPoint.transform.position - snapPoint.transform.rotation * snapAnchorOffset;
        snappedPoint = snapPoint;
    }

    void Unsnap()
    {
        snappedPoint = null;
        Camera cam = Camera.main;
        if (cam != null)
        {
            float dist = Vector3.Distance(cam.transform.position, transform.position);
            buildManager.placementDistance = Mathf.Clamp(dist, buildManager.placementDistanceMin, buildManager.placementDistanceMax);
        }
        transform.position = buildManager.GetViewCenterWorldPosition(buildManager.placementDistance);
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;

        return new Vector3(x, y, z);
    }

    void SnapRotation()
    {
        float yaw = transform.eulerAngles.y;
        float pitch = transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        float snappedYaw = Mathf.Round(yaw / snapRotationAngle) * snapRotationAngle;
        float snappedPitch = Mathf.Round(pitch / snapRotationAngle) * snapRotationAngle;
        snappedPitch = Mathf.Clamp(snappedPitch, -pitchClamp, pitchClamp);

        transform.rotation = Quaternion.Euler(snappedPitch, snappedYaw, 0f);
    }

    void CheckPlacementValidity()
    {
        isValidPlacement = true;

        if (itemCollider == null) return;

        Collider[] overlappingColliders = Physics.OverlapBox(
            itemCollider.bounds.center,
            itemCollider.bounds.extents,
            transform.rotation
        );

        foreach (Collider collider in overlappingColliders)
        {
            if (collider != itemCollider && !collider.isTrigger)
            {
                BuildableItem otherItem = collider.GetComponent<BuildableItem>();
                if (otherItem != null && otherItem.isPlaced)
                {
                    isValidPlacement = false;
                    break;
                }
            }
        }
    }

    void UpdateVisualFeedback(bool isValid, bool isSnapped)
    {
        if (itemRenderer == null) return;

        if (isSnapped)
        {
            itemRenderer.material = buildManager.snappedMaterial;
        }
        else if (isValid)
        {
            itemRenderer.material = buildManager.validMaterial;
        }
        else
        {
            itemRenderer.material = buildManager.invalidMaterial;
        }
    }

    public void PlaceItem()
    {
        isPlaced = true;

        if (itemRenderer != null && originalMaterial != null)
        {
            itemRenderer.material = originalMaterial;
        }

        Collider[] allCols = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in allCols) c.isTrigger = false;

        Rigidbody[] allRbs = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody r in allRbs) r.isKinematic = true;

        Debug.Log("??????????: " + gameObject.name);
    }

    public void CancelBuild()
    {
        if (snappedPoint != null)
        {
            snappedPoint.isOccupied = false;
            snappedPoint = null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (itemRenderer != null && originalMaterial != null)
        {
            itemRenderer.material = originalMaterial;
        }
    }

    void OnDestroy()
    {
        if (snappedPoint != null)
        {
            snappedPoint.isOccupied = false;
        }
    }
}