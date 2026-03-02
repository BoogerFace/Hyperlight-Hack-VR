using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_References : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public float pathUpdateDelay = 0.2f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
