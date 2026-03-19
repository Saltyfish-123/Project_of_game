using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavPathLineRenderer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float pathHeightOffset = 0.3f;
    [SerializeField] private float DrawUpdateSpeed = 0.2f;

    private Coroutine drawCouroutine;

    private void Start()
    {
        if (drawCouroutine != null)
        {
            StopCoroutine(drawCouroutine);

        }
        drawCouroutine = StartCoroutine(DrawNavMeshLine());
    }

    private IEnumerator DrawNavMeshLine()
    {
        WaitForSeconds Wait = new WaitForSeconds(DrawUpdateSpeed);
        NavMeshPath path = new NavMeshPath();

        while (target != null)
        {
            if (NavMesh.CalculatePath(player.position, target.position, NavMesh.AllAreas, path))
            {
                line.positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    line.SetPosition(i, path.corners[path.corners.Length - 1 - i] + Vector3.up * pathHeightOffset);
                }
                yield return Wait;
            }
        }
    }


}