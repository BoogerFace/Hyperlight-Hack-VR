using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Animator anim;

    public int gearValue = 100; // how many gears this enemy is worth when destroyed

    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerMelee"))
        {
            TakeDamage(20); // Example melee damage
        }
        else if (other.CompareTag("PlayerProjectile"))
        {
            TakeDamage(10); // Example projectile damage
            Destroy(other.gameObject); // Destroy projectile after hit
        }

    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // ✅ No damage after death
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (isDead) return; // ✅ Safety check
        isDead = true; // ✅ Mark enemy as dead
        // Give gears to player
        GearManager.instance.AddGears(gearValue);
        anim.SetTrigger("Die");

        // Destroy this enemy
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20);
        }


    }
}
