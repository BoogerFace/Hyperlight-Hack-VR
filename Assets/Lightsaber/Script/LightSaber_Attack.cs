using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber_Attack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 3; // Damage dealt on hit
    public GameObject sparkEffectPrefab; // Assign your spark particle prefab in Inspector
    public float sparkLifetime = 2f; // Lifetime of spark effect

    [Header("Valid Target Tags")]
    public string[] validTags = { "Enemy", "Melee", "Boss" };

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject, other.ClosestPoint(transform.position), Vector3.up);
    }

    private void HandleCollision(GameObject target, Vector3 hitPoint, Vector3 hitNormal)
    {
        // ✅ Check if target has a valid tag
        foreach (string tag in validTags)
        {
            if (target.CompareTag(tag))
            {
                // 1. Spawn spark effect at the hit position
                if (sparkEffectPrefab != null)
                {
                    GameObject spark = Instantiate(sparkEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
                    Destroy(spark, sparkLifetime);
                }

                // 2. Try to apply damage if the target has a health script
                var bossAI = target.GetComponent<EnemyBossAI>();
                if (bossAI != null)
                {
                    bossAI.TakeDamage(damageAmount);
                }

                var meleeEnemy = target.GetComponent<EnemyMeleeAI>(); // Example if you have melee AI script
                if (meleeEnemy != null)
                {
                    meleeEnemy.TakeDamage(damageAmount);
                }

                var genericHealth = target.GetComponent<Enemy_Health>(); // In case enemies use a generic PlayerHealth script
                if (genericHealth != null)
                {
                    genericHealth.TakeDamage(damageAmount);
                }

                break; // ✅ Stop checking after first valid tag match
            }
        }
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
