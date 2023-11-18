using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Fighter : MonoBehaviour
{
    [SerializeField] private Env env;

    private GameObject closestCharacter;

    private NavMeshAgent NPC;

    //Attacking
    private float timeBetweenAttacks;
    private bool alreadyAttacked;

    //States
    [SerializeField] private float sightRange, attackRange;

    private void Awake()
    {
        NPC.GetComponent<NavMeshAgent>();
    }

    // Put GetToClosestAgent in the fixed update
    private void FixedUpdate()
    {
        closestCharacter = env.GetClosestCharacter(env.characters, transform.localPosition);

        if(closestCharacter != null)
        {
            float dist = Vector3.Distance(transform.localPosition, closestCharacter.transform.localPosition);

            if (dist <= sightRange && dist > attackRange) ChasePlayer();
            else if (dist <= sightRange && dist <= attackRange) AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        if (closestCharacter != null) NPC.SetDestination(closestCharacter.transform.localPosition);
    }

    private void AttackPlayer()
    {
        // Make sure the npc doesn't move
        NPC.SetDestination(transform.localPosition);

        transform.LookAt(closestCharacter.transform);
    }
}
