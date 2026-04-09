using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodGenerator : MonoBehaviour
{
    [Header("木板设置")]
    public GameObject woodPrefab;      // 木板预制体
    public Transform generationPoint;   // 木板生成位置
    public Transform destroyPoint;     // 木板消失位置
    public float moveSpeed = 5f;       // 木板移动速度
    public float generationInterval = 2f; // 生成间隔
    public float woodWidth = 2f;       // 木板宽度
    public int maxWoodCount = 5;       // 同时存在的最大木板数量

    [Header("木板池设置")]
    public int poolSize = 10;          // 对象池大小

    [Header("运动方向")]
    public Vector3 moveDirection = Vector3.back; // 默认向后移动

    private List<GameObject> woodPool = new List<GameObject>();
    private Queue<GameObject> activeWoods = new Queue<GameObject>();
    private float timer = 0f;
    private bool isActive = true;

    void Start()
    {
        // 初始化对象池
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject wood = Instantiate(woodPrefab, generationPoint.position, generationPoint.rotation);
            wood.SetActive(false);
            woodPool.Add(wood);
        }
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        // 定时生成木板
        if (timer >= generationInterval && activeWoods.Count < maxWoodCount)
        {
            GenerateWood();
            timer = 0f;
        }

        // 移动所有活动的木板
        MoveActiveWoods();

        // 检查是否需要销毁木板
        CheckDestroyWoods();
    }

    void GenerateWood()
    {
        // 从对象池获取可用木板
        GameObject wood = GetPooledWood();
        if (wood == null) return;

        // 重置木板位置和状态
        wood.transform.position = generationPoint.position;
        wood.transform.rotation = generationPoint.rotation;
        wood.SetActive(true);

        // 添加到活动队列
        activeWoods.Enqueue(wood);

        // 播放生成效果
        ParticleSystem ps = wood.GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Play();

    }

    GameObject GetPooledWood()
    {
        foreach (GameObject wood in woodPool)
        {
            if (!wood.activeInHierarchy)
            {
                return wood;
            }
        }

        // 如果对象池已满，创建新的
        GameObject newWood = Instantiate(woodPrefab, generationPoint.position, generationPoint.rotation);
        newWood.SetActive(false);
        woodPool.Add(newWood);
        return newWood;
    }

    void MoveActiveWoods()
    {
        // 创建一个临时队列来存储需要移除的木板
        Queue<GameObject> newQueue = new Queue<GameObject>();

        while (activeWoods.Count > 0)
        {
            GameObject wood = activeWoods.Dequeue();
            if (wood != null && wood.activeSelf)
            {
                // 沿着指定方向移动
                wood.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
              
                newQueue.Enqueue(wood);
            }
        }

        // 更新活动队列
        activeWoods = newQueue;
    }

    void CheckDestroyWoods()
    {
        if (activeWoods.Count == 0) return;

        // 检查最前面的木板是否到达销毁点
        GameObject frontWood = activeWoods.Peek();
        if (frontWood != null && frontWood.activeSelf)
        {
            // 计算与销毁点的距离
            float distance = Vector3.Distance(frontWood.transform.position, destroyPoint.position);

            // 如果是朝后移动，检查Z轴距离
            if (moveDirection == Vector3.back)
            {
                distance = Mathf.Abs(frontWood.transform.position.z - destroyPoint.position.z);
            }

            if (distance < 0.5f)
            {
                ReturnWoodToPool(activeWoods.Dequeue());
            }
        }
    }

    void ReturnWoodToPool(GameObject wood)
    {
        if (wood != null)
        {
            wood.SetActive(false);

            // 播放消失特效
            StartCoroutine(PlayDestroyEffect(wood));
        }
    }

    IEnumerator PlayDestroyEffect(GameObject wood)
    {
        if (wood != null)
        {
            ParticleSystem ps = wood.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // 外部控制方法
    public void SetActive(bool active) => isActive = active;
    public void SetSpeed(float speed) => moveSpeed = speed;
    public void SetInterval(float interval) => generationInterval = interval;
}
