using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyMelee_Test : MonoBehaviour
{
    public float meleeRange;
    public bool playerInRange;
    public LayerMask whatIsPlayer;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, meleeRange, whatIsPlayer); // 🔹 NEW Check if the player is in melee range

        if ( playerInRange)
        {
            Debug.Log("Player in melee range");
            anim.SetTrigger("MeleeAttack");

        }


    }
}
