using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;

    public HealthBar healthBar;

    private bool isDead = false;    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        healthBar.SetSliderMax(maxHealth);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            takeDamage(20f);
        }


    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);

        if (currentHealth < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        // Inform GearManager to handle Continue/GameOver logic
        if (GearManager.instance != null)
        {
            GearManager.instance.PlayerDied();
        }
    }

    public void HealToFull()
    {
        isDead = false;
        currentHealth = maxHealth;
        healthBar.SetSlider(currentHealth);
        Debug.Log("Player revived with full health!");
    }
}
