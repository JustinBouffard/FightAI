using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Fighter : MonoBehaviour
{
    [SerializeField] private Env env;

    private GameObject closestCharacter;

    private Animator animator;

    private AttackArea attackArea;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttacked;

    //States
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private float speed;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackArea = GameObject.Find("AttackArea").GetComponent<AttackArea>();
    }

    // Put GetToClosestAgent in the fixed update
    private void FixedUpdate()
    {
        closestCharacter = env.GetClosestCharacter(env.characters, transform.localPosition);

        if(closestCharacter != null)
        {
            float dist = Vector3.Distance(transform.position, closestCharacter.transform.position);

            if (dist <= sightRange && dist > attackRange) ChasePlayer();
            else if (dist <= sightRange && dist <= attackRange) AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        if (closestCharacter != null) transform.position = Vector3.MoveTowards(transform.position, closestCharacter.transform.position, speed);
    }

    private void AttackPlayer()
    {
        // Make sure the npc doesn't move
        //transform.position = Vector3.MoveTowards(transform.position, closestCharacter.transform.position, speed);

        transform.LookAt(closestCharacter.transform);

        if(!alreadyAttacked)
        {
            animator.Play(name = "Sword And Shield Slash");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
