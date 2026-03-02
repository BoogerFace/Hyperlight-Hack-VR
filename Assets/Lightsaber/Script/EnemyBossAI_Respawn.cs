using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossAI_Respawn : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public Transform gunPoint;
    public Transform player;
    public Transform boss;

    public LayerMask whatIsGround, whatIsPlayer;
    public Collider melee;

    public float damage;

    [Header("Boss Rewards")]
    public int gearValue = 500;

    [Header("Boss Health")]
    public int maxHealth = 100;
    private int health;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange, meleeRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool playerInMeleeRange;

    private Animator anim;
    private bool isDead = false;

    public AudioSource audioSource;
    public AudioClip shootSound;

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20);
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            anim.SetBool("MeleeAttack", false);
            anim.SetBool("Attack", false);
            Patroling();
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            anim.SetBool("MeleeAttack", false);
            anim.SetBool("Attack", false);
            ChasePlayer();
            FacePlayer();
        }

        if (playerInAttackRange && playerInSightRange && !playerInMeleeRange)
        {
            melee.enabled = false;
            anim.SetBool("MeleeAttack", false);
            AttackPlayer();
        }

        if (playerInMeleeRange)
        {
            melee.enabled = true;
            anim.SetBool("MeleeAttack", true);
            FacePlayer();
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
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
            agent.SetDestination(walkPoint);
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(player.position);
        FacePlayer();

        if (!alreadyAttacked)
        {
            anim.SetBool("Attack", true);

            Vector3 targetDirection = (player.position + Vector3.up * 1.2f) - gunPoint.position;
            targetDirection.Normalize();
            audioSource.PlayOneShot(shootSound);
            Rigidbody rb = Instantiate(projectile, gunPoint.position, Quaternion.LookRotation(targetDirection)).GetComponent<Rigidbody>();
            rb.velocity = targetDirection * 32f;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        anim.SetTrigger("Die");
        melee.enabled = false;

        if (GearManager.instance != null)
        {
            GearManager.instance.AddGears(gearValue);
        }

        // Start respawn coroutine
        StartCoroutine(RespawnAfterDelay(30f));

    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset state
        isDead = false;
        health = maxHealth;

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        if (melee != null)
            melee.enabled = true;

        // Play respawn animation if available
        if (anim != null)
        {
            anim.ResetTrigger("Die");
        }

        Debug.Log("Boss has respawned!");
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.CompareTag("PlayerMelee"))
        {
            TakeDamage(10);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
