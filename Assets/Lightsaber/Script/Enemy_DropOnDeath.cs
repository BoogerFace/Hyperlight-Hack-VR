using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DropOnDeath : MonoBehaviour
{
    [Header("Drop Settings")]
    public GameObject bombPrefab;   // Prefab to drop on death
    public Transform dropPoint;     // Optional: where the bomb spawns
    public float dropDelay = 0.5f;  // Delay before dropping (so it looks natural)

    private EnemyMeleeAI enemy;     // Reference to the AI script
    private bool hasDropped = false;

    void Awake()
    {
        enemy = GetComponent<EnemyMeleeAI>();
    }

    void Update()
    {
        if (enemy == null) return;

        // Check if enemy has died
        if (enemy != null && enemyHealthZero() && !hasDropped)
        {
            hasDropped = true;
            Invoke(nameof(DropBomb), dropDelay);
        }
    }

    private bool enemyHealthZero()
    {
        // Access EnemyMeleeAI's internal state
        return enemy != null && enemyHealthField() <= 0;
    }

    private int enemyHealthField()
    {
        return enemy.health; // directly use health field from EnemyMeleeAI
    }

    private void DropBomb()
    {
        if (bombPrefab == null) return;

        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position;

        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


}
