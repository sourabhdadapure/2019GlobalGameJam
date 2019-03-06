using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float directionFacing;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInfViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInfViewRadius.Length; i++)
        {

            Transform target = targetsInfViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    Debug.Log(visibleTargets.Count);

                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angDeg, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angDeg += directionFacing;
        }
        return new Vector3(Mathf.Sin(angDeg * Mathf.Deg2Rad),
            Mathf.Cos(angDeg * Mathf.Deg2Rad), 0);
    }
};
