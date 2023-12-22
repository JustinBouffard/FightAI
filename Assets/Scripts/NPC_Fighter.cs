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
    [SerializeField] private AttackArea attackArea;
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
    [HideInInspector] public float damage;
    private bool canBeHit = true;
    [Space(15f)]

    //Random Spawning
    [Header("Random Spawning")]
    [SerializeField] private float xMinValueSpawning;
    [SerializeField] private float xMaxValueSpawning;
    [SerializeField] private float zMinValueSpawning;
    [SerializeField] private float zMaxValueSpawning;

    Vector3 initalPosition;

    bool canChase = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        randomSpawn = GetComponent<RandomSpawning>();
    }

    private void Start()
    {
        animator.Play(name = "Sword And Shield Slash");
        damage = health / numOfHits;

        attackArea.gameObject.active = false;

        initalPosition = transform.localPosition;
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
            canChase = false;
            env.AddAgent();
            health = 1f;

            transform.localPosition = initalPosition;

            //randomSpawn.MoveToSafeRandomPosition(xMinValueSpawning, xMaxValueSpawning, zMinValueSpawning, zMaxValueSpawning);
            //transform.localPosition = randomSpawn.localPosition;
            //transform.localRotation = randomSpawn.localRotation;
        }

        HasKilled(attackArea.hasKilled);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BlueSwordHit"))
        {
            if(canBeHit)
            {
                TakingDamage();
                StartCoroutine(canBeHitDelay());
            }
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
        if (!canChase)
        {
            StartCoroutine(ChaseDelay());
        }
        else if (canChase)
        {
            transform.position = Vector3.MoveTowards(transform.position, closestCharacter.transform.position, speed);
        }
    }

    private void AttackPlayer()
    {
        // Make sure the npc doesn't move

        transform.LookAt(closestCharacter.transform);

        if(!alreadyAttacked && timeBetweenAttacks != 0)
        {
            isAttacking = true;

            animator.Play(name = "Sword And Shield Slash 0");
            attackArea.gameObject.active = true;

            alreadyAttacked = true;

            StartCoroutine(isAttackingDelay());
            StartCoroutine(AttackDelay());
        }
    }

    private void HasKilled(bool hasKilled)
    {
        if (hasKilled)
        {
            hasKilled = false;
            env.AgentsCount--;
        }
    }

    IEnumerator ChaseDelay()
    {
        yield return new WaitForSeconds(4f);
        yield return canChase = true;
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
        yield return attackArea.gameObject.active = false;
        yield return isAttacking = false;
    }
}
