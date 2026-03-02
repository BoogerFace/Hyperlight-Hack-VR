using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Turret : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;   // Prefab to shoot
    public Transform firePoint;           // Point where projectile spawns
    public float projectileSpeed = 20f;   // Speed of projectile
    public float fireRate = 0.5f;         // Time between shots (0.5s = 2 shots/sec)

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Apply forward velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }

        // Auto destroy projectile after 5 seconds (prevents clutter in scene)
        Destroy(projectile, 5f);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
}
