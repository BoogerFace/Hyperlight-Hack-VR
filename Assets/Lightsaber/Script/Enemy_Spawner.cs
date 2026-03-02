using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;    // Enemy prefab to spawn
    public Transform spawnPoint;      // Where enemies will appear
    public float spawnInterval = 5f;  // Time between spawns
    public int maxSpawns = 10;        // How many to spawn total (-1 = infinite)
    public int spawnAmount = 3;       // How many enemies per cycle

    [Header("Player Reference")]
    public Transform player;

    private bool playerInside = false;
    private int spawnCount = 0;

    private void SpawnEnemyWave()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                // Small random offset so they don’t overlap perfectly
                Vector3 offset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position + offset, spawnPoint.rotation);

                // Inject player into EnemyMeleeAI
                EnemyMeleeAI meleeAI = enemy.GetComponent<EnemyMeleeAI>();
                if (meleeAI != null) meleeAI.player = player;

                // Inject player into EnemyBossAI
                EnemyBossAI bossAI = enemy.GetComponent<EnemyBossAI>();
                if (bossAI != null) bossAI.player = player;

                // Inject player into Shooter AI
                EnemyBrain_Stupid shooter = enemy.GetComponent<EnemyBrain_Stupid>();
                if (shooter != null) shooter.target = player;

                spawnCount++;
            }
        }
        else
        {
            Debug.LogWarning("Spawner missing prefab or spawn point!");
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (playerInside && (maxSpawns < 0 || spawnCount < maxSpawns))
        {
            SpawnEnemyWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            Debug.Log("Player entered spawner area");
            playerInside = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
