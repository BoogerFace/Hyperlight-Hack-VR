using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBase : MonoBehaviour
{
    protected Transform player; // Shared player reference

    public virtual void SetTarget(Transform target)
    {
        player = target;
    }
}
