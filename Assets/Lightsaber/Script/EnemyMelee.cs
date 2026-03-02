using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour

{
    public float damage;
    public float damageInterval;   // How often damage is applied (seconds)

    private bool playerInside = false;
    private Coroutine damageCoroutine;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy Melee Triggered");

        if (other.CompareTag("Player"))
        {
            playerInside = true;
            // Start damaging over time
            damageCoroutine = StartCoroutine(DamageOverTime(other.GetComponent<PlayerHealth>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            // Stop damaging
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime(PlayerHealth playerHealth)
    {
        while (playerInside && playerHealth != null)
        {
            playerHealth.takeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


    }
}
