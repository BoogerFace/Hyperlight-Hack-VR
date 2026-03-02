using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain_Stupid : MonoBehaviour
{

    public Transform target;

    public Enemy_References enemyReferences;

    private float shootingDistance;

    private float pathUpdateDeadLine;

    private void Awake()
    {
        enemyReferences = GetComponent<Enemy_References>();

    }
    // Start is called before the first frame update
    void Start()
    {
        shootingDistance  = enemyReferences.navMeshAgent.stoppingDistance;

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) 
        {
            bool inRange = Vector3.Distance(transform.position, target.position) <= shootingDistance;

            if (inRange)
            {
                lookAtTarget();
            }
            else
            {
                updatePath();
            }
            enemyReferences.animator.SetBool("Shooting", !inRange);
        }
        enemyReferences.animator.SetFloat("Speed", enemyReferences.navMeshAgent.desiredVelocity.sqrMagnitude);
    }

    private void lookAtTarget()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);

    }

    private void updatePath()
    {
        if (Time.time >= pathUpdateDeadLine)
        {
            Debug.Log("Update Path");
            pathUpdateDeadLine = Time.time + enemyReferences.pathUpdateDelay;
            enemyReferences.navMeshAgent.SetDestination(target.position);
        }


    }

    private void OnDrawGizmos()
    {
        if (enemyReferences != null && enemyReferences.navMeshAgent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyReferences.navMeshAgent.stoppingDistance);
        }
    }
}
