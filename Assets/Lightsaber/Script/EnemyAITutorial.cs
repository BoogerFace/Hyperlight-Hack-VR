using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAITutorial : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component

    public Transform player; // Reference to the player's transform

    public LayerMask whatIsGround, whatIsPlayer; // Layer masks for ground and player detection

    public int health; // Health of the enemy

    //Patrolling
    public Vector3 walkPoint; // The point the enemy will walk to
    bool walkPointSet; // Whether the walk point has been set
    public float walkPointRange; // Range within which the enemy can walk

    //Attacking
    public float timeBetweenAttacks; // Time between attacks
    bool alreadyAttacked; // Whether the enemy has already attacked
    public GameObject projectile; // Prefab for the projectile to be fired when attacking

    //States
    public float sightRange, attackRange; // Ranges for sight and attack
    public bool playerInSightRange, playerInAttackRange; // Whether the player is in sight or attack range

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Awake()
    {
        //player = GameObject.Find("Player").transform; // Find the player object in the scene
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to this enemy
    }

    private void Update()
    {
        //check sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); // Check if the player is in sight range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); // Check if the player is in attack range
    
        if (!playerInSightRange && !playerInAttackRange) // If the player is not in sight or attack range
        {
            Patroling(); // Call the patrolling method
        }


        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            FacePlayer(); // Keep facing the player while chasing
        }

        if (playerInAttackRange && playerInSightRange) // If the player is in attack range and sight range
        {
            Debug.Log("Player in attack range"); // Log that the player is in attack range
            AttackPlayer(); // Call the attack player method
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint(); // If the walk point is not set, search for a new walk point
        }

        if(walkPointSet) // If the walk point is set
        {
            agent.SetDestination(walkPoint); // Set the agent's destination to the walk point
            // Check if the agent has reached the walk point
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f) // If the distance to the walk point is less than 1 unit
            {
                walkPointSet = false; // Reset the walk point set flag
            }

            //walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
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
        agent.SetDestination(transform.position);

        transform.LookAt(player); // Make the enemy look at the player

        if(!alreadyAttacked)
        {
            Debug.Log("Attack Player"); // Log the attack action to the console
            //attack code here, all the different types of attacks also
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>(); // Instantiate the projectile prefab
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse); // Add force to the projectile to make it move towards the player
            rb.AddForce(transform.up * 8f, ForceMode.Impulse); // Add upward force to the projectile

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
        health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.1f); // Destroy the enemy after a short delay
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject); // Destroy the enemy game object
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Set the color for the Gizmos
        Gizmos.DrawWireSphere(transform.position, attackRange); // Draw a wire sphere for sight range
        Gizmos.color = Color.yellow; // Set the color for the Gizmos
        Gizmos.DrawWireSphere(transform.position, sightRange); // Draw a wire sphere for attack range
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

}
