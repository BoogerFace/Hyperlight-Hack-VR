using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component

    public Transform gunPoint; // Reference to the gun point from where projectiles will be fired

    public Transform player; // Reference to the player's transform

    public Transform boss; // Reference to the boss's transform

    public LayerMask whatIsGround, whatIsPlayer; // Layer masks for ground and player detection

    public Collider melee;

    public float damage;

    [Header("Boss Rewards")]
    public int gearValue = 500; // 🔹 How many gears boss gives when killed

    public int health = 100;

    //Patrolling
    public Vector3 walkPoint; // The point the enemy will walk to
    bool walkPointSet; // Whether the walk point has been set
    public float walkPointRange; // Range within which the enemy can walk

    //Attacking
    public float timeBetweenAttacks; // Time between attacks
    bool alreadyAttacked; // Whether the enemy has already attacked
    public GameObject projectile; // Prefab for the projectile to be fired when attacking

    //States
    public float sightRange, attackRange, meleeRange; // Ranges for sight and attack
    public bool playerInSightRange, playerInAttackRange; // Whether the player is in sight or attack range
    public bool playerInMeleeRange; // Whether the player is in melee range 

    private Animator anim; // Reference to the Animator component for animations

    private bool isDead = false; // Flag to check if the enemy is dead

    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip shootSound; // Sound to play on death


    private void Awake()
    {
        //player = GameObject.Find("Player").transform; // Find the player object in the scene
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to this enemy
        anim = GetComponent<Animator>(); // Grab animator on this object
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

    private void Update()
    {

        if (isDead) return; // If the enemy is dead, do not execute further logic

        if (isDead) return; // If the enemy is dead, do not execute further logic

        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20);
        }


        else
        {


            //check sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); // Check if the player is in sight range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); // Check if the player is in attack range
            playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, whatIsPlayer); // 🔹 NEW Check if the player is in melee range

            if (!playerInSightRange && !playerInAttackRange) // If the player is not in sight or attack range
            {
                anim.SetBool("MeleeAttack", false); // reset melee animation
                anim.SetBool("Attack", false);
                Debug.Log("Patrolling"); // Log that the enemy is patrolling
                Patroling(); // Call the patrolling method
                anim.SetFloat("Speed", agent.velocity.magnitude);
            }


            if (playerInSightRange && !playerInAttackRange)
            {
                anim.SetBool("MeleeAttack", false); // reset melee animation
                anim.SetBool("Attack", false);
                Debug.Log("Chasing Player"); // Log that the enemy is chasing the player
                ChasePlayer();
                FacePlayer(); // Keep facing the player while chasing
            }

            if (playerInAttackRange && playerInSightRange && !playerInMeleeRange) // If the player is in attack range and sight range
            {
                melee.enabled = false;
                anim.SetBool("MeleeAttack", false); // reset melee animation
                Debug.Log("Player in attack range"); // Log that the player is in attack range
                AttackPlayer(); // Call the attack player method

            }

            //if (playerInMeleeRange && playerInAttackRange)
            //{
            //    Debug.Log("Player in melee range");
            //    anim.SetTrigger("MeleeAttack");
            //}

            if (playerInMeleeRange) // 🔹 NEW
            {
                melee.enabled = true;

                Debug.Log("Melee Animation");


                anim.SetBool("MeleeAttack", true); // trigger melee animation
                FacePlayer();
            }

            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint(); // If the walk point is not set, search for a new walk point
        }

        if (walkPointSet) // If the walk point is set
        {
            agent.SetDestination(walkPoint); // Set the agent's destination to the walk point
            // Check if the agent has reached the walk point
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f) // If the distance to the walk point is less than 1 unit
            {
                walkPointSet = false; // Reset the walk point set flag
            }


        }
    }

    private void SearchWalkPoint()
    {
        // Calculate a random point within the walk point range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        // Set the walk point to a new position based on the random values
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // Check if the walk point is on the ground

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) // If the walk point is valid
        {
            walkPointSet = true; // Set the walk point as set
            agent.SetDestination(walkPoint); // Move the agent to the walk point
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position); // Set the agent's destination to the player's position causes them to go after the player

    }

    private void AttackPlayer()
    {
        //make sure enemy doesn't move while attacking
        agent.SetDestination(player.position);
        FacePlayer();


        //transform.LookAt(player); // Make the enemy look at the player


        if (!alreadyAttacked)
        {

            Debug.Log("Attack Player"); // Log the attack action to the console
            //attack code here, all the different types of attacks also

            anim.SetBool("Attack", true); // Trigger the attack animation

            Vector3 targetDirection = (player.position + Vector3.up * 1.2f) - gunPoint.position;
            targetDirection.Normalize();
            audioSource.PlayOneShot(shootSound);
            Rigidbody rb = Instantiate(projectile, gunPoint.position, Quaternion.LookRotation(targetDirection)).GetComponent<Rigidbody>();
            rb.velocity = targetDirection * 32f; // projectile speed
            //Rigidbody rb = Instantiate(projectile, gunPoint.position, Quaternion.identity).GetComponent<Rigidbody>(); // Instantiate the projectile prefab
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse); // Add force to the projectile to make it move towards the player
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse); // Add upward force to the projectile

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // Reset the attack after a certain time
        }
    }


    private void ResetAttack()
    {
        alreadyAttacked = false; // Reset the already attacked flag

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
        if (isDead) return; // safety check
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

        // Destroy after 5 seconds (enough time for animation to play)
        Destroy(gameObject, 5f);
        //isDead = true;
        //agent.isStopped = true;

        ////  Play death animation
        //anim.SetTrigger("Die");

        ////  Destroy after 10 seconds
        //Destroy(gameObject, 5f);
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
        Gizmos.color = Color.red; // Set the color for the Gizmos
        Gizmos.DrawWireSphere(transform.position, attackRange); // Draw a wire sphere for sight range
        Gizmos.color = Color.yellow; // Set the color for the Gizmos
        Gizmos.DrawWireSphere(transform.position, sightRange); // Draw a wire sphere for attack range
        Gizmos.color = Color.blue; // 🔹 NEW Set the color for the Gizmos
        Gizmos.DrawWireSphere(transform.position, meleeRange); // 🔹 NEW Draw a wire sphere for melee range
    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
