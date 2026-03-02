using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Collider melee;  // the collider on weapon/hand for hit detection
    public Animator anim;

    [Header("Stats")]
    public int health = 100;
    public float walkPointRange = 10f;
    public float sightRange = 10f;
    public float meleeRange = 2f;
    public int meleeDamage = 20;
    public float timeBetweenAttacks = 2f;
    public int gearValue = 5;

    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool isDead = false;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (melee != null) melee.enabled = false; // disable by default
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep only the horizontal direction
        if (direction.sqrMagnitude > 0.01f) // Avoid zero rotation
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);
        }
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.2f);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        bool playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, whatIsPlayer);

        if (!playerInSightRange && !playerInMeleeRange)
        {
            melee.enabled = false; // enable hit detection
            anim.SetBool("MeleeAttack", false); // reset melee animations
            Patrol();
        }
        if (playerInSightRange && !playerInMeleeRange)
        {
            melee.enabled = false; // enable hit detection
            anim.SetBool("MeleeAttack", false); // reset melee animation
            FacePlayer();
            ChasePlayer();
        }

        if (playerInMeleeRange)
        {

            melee.enabled = true; // enable hit detection

            anim.SetBool("MeleeAttack", true);
            FacePlayer();
        }
        anim.SetFloat("Speed", agent.velocity.magnitude);

    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // --- Combat ---
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop NavMeshAgent completely
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false; // ✅ prevents further movement updates
        }

        // Play death animation
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        if (GearManager.instance != null)
        {
            GearManager.instance.AddGears(gearValue);
        }
        melee.enabled = false;

        Destroy(gameObject, 5f); // remove after animation
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.CompareTag("PlayerMelee"))
        {
            TakeDamage(10); // Example damage value
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }


}
