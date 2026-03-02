using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;        // First point
    public Transform pointB;        // Second point
    public float speed = 2f;        // Movement speed
    public float pauseTime = 1f;    // Pause duration at each point
    public float damage = 10f;      // Damage to inflict on player

    private Vector3 target;         // Current target
    private bool isMoving = true;   // Controls pause state


    void Start()
    {
        if (pointA != null && pointB != null)
        {
            target = pointB.position;
        }
    }

    void Update()
    {
        if (pointA == null || pointB == null || !isMoving) return;

        // Smooth movement using MoveTowards
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if reached the target
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            StartCoroutine(PauseAndSwitch());
        }
    }

    private IEnumerator PauseAndSwitch()
    {
        isMoving = false; // stop moving
        yield return new WaitForSeconds(pauseTime); // wait

        // Switch target after pause
        target = (target == pointA.position) ? pointB.position : pointA.position;
        isMoving = true; // resume movement
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player has a script with a method 'TakeDamage'
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(damage); // Inflict 10 damage
            }
        }
    }

}
