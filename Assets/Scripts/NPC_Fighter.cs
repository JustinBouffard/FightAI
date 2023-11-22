using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Fighter : MonoBehaviour
{
    // Script variables
    [Header("Script Variables")]
    [SerializeField] private Env env;
    [SerializeField] private FightAgent fightAgent;
    private Animator animator;
    private RandomSpawning randomSpawn;
    [Space(15f)]

    private GameObject closestCharacter;

    //Attacking
    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    [HideInInspector] public bool isAttacking = false;
    private bool alreadyAttacked;
    [Space(15f)]

    //States
    [Header("States")]
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private float speed;
    [Space(15f)]

    //Health
    [Header("Health")]
    [SerializeField] private int numOfHits;
    public float health = 1.0f;
    private float damage;
    private bool canBeHit = true;
    [Space(15f)]

    //Random Spawning
    [Header("Random Spawning")]
    [SerializeField] private float xMinValueSpawning;
    [SerializeField] private float xMaxValueSpawning;
    [SerializeField] private float zMinValueSpawning;
    [SerializeField] private float zMaxValueSpawning;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        randomSpawn = GetComponent<RandomSpawning>();
    }

    private void Start()
    {
        animator.Play(name = "Sword And Shield Slash");
        damage = health / numOfHits;
    }

    private void FixedUpdate()
    {
        closestCharacter = env.GetClosestCharacter(env.characters, transform.localPosition);
        transform.LookAt(closestCharacter.transform);

        if (closestCharacter != null)
        {
            float dist = Vector3.Distance(transform.position, closestCharacter.transform.position);

            if (dist <= sightRange && dist > attackRange) ChasePlayer();
            else if (dist <= sightRange && dist <= attackRange) AttackPlayer();
        }

        if (fightAgent.episodeBegin)
        {
            fightAgent.episodeBegin = false;
            env.AddAgent();
            health = 1f;

            randomSpawn.MoveToSafeRandomPosition(xMinValueSpawning, xMaxValueSpawning, zMinValueSpawning, zMaxValueSpawning);
            transform.localPosition = randomSpawn.localPosition;
            transform.localRotation = randomSpawn.localRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwordHit") && canBeHit)
        {
            TakingDamage();
            StartCoroutine(canBeHitDelay());
        }
    }

    private void TakingDamage()
    {
        if (health > damage)
        {
            health -= damage;
            canBeHit = false;
        }
        else env.AgentsCount--;
    }

    private void ChasePlayer()
    {
        if (closestCharacter != null) transform.position = Vector3.MoveTowards(transform.position, closestCharacter.transform.position, speed);
    }

    private void AttackPlayer()
    {
        // Make sure the npc doesn't move

        transform.LookAt(closestCharacter.transform);

        if(!alreadyAttacked)
        {
            isAttacking = true;
            animator.Play(name = "Sword And Shield Slash 0");

            alreadyAttacked = true;

            StartCoroutine(isAttackingDelay());
            StartCoroutine(AttackDelay());
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        yield return alreadyAttacked = false;
    }

    IEnumerator canBeHitDelay()
    {
        yield return new WaitForSeconds(0.7f);
        yield return canBeHit = true;
    }

    IEnumerator isAttackingDelay()
    {
        yield return new WaitForSeconds(0.7f);
        yield return isAttacking = false;
    }
}
