using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Animations;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine.WSA;
using UnityEngine.PlayerLoop;

public class FightAgent : Agent
{
    //VARIABLES

    //Health variables
    [Header("Health")]
    [SerializeField] public float health;
    [SerializeField] private int numOfHits;
    [HideInInspector] public float damage;
    private float initialHealth;
    [Space(15)]

    // Movement
    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float yawRotationSpeed = 3f;
    [Space(15)]

    // Random spawning
    [Header("Random Spawning")]
    [SerializeField] float zMaxValueSpawning;
    [SerializeField] float zMinValueSpawning;
    [SerializeField] float xMaxValueSpawning;
    [SerializeField] float xMinValueSpawning;
    [Space(15)]

    // Scripts variables
    [Header("Scripts Variables")]
    [SerializeField] Env env;
    Animator animator;
    AttackArea attackArea;
    RandomSpawning randomSpawn;
    [Space(15)]

    // Combat conditions
    [Header("Combat Conditions")]
    [SerializeField] private float sphereCastRadius;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isBlocking;
    bool canAttack = true;
    bool canBlock = true;
    bool canMove = true;
    bool canRotate = true;
    bool canBeHit = true;
    [Space(15)]

    //Stamina
    [SerializeField] Stamina stamina;

    // Rotation
    private float smoothYawRotation = 1f;

    [HideInInspector] public GameObject closestCharacter;

    [HideInInspector] public bool episodeBegin = false;

    private int blockAction = -1;
    bool isInFrontOf = false;

    Vector3 initalPosition;
    Quaternion initalRotation;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackArea = GameObject.Find("AttackArea").GetComponent<AttackArea>();

        attackArea.gameObject.active = false;

        randomSpawn = GetComponent<RandomSpawning>();
    }
    private void Start()
    {
        initialHealth = health;

        damage = health / numOfHits;

        initalPosition = transform.localPosition;
        initalRotation = transform.localRotation;
    }

    private void Update()
    {
        Animating();

        HasHit(attackArea.hasHit);
        HasKilled(attackArea.hasKilled);
        attackArea.hasHit = false;
        attackArea.hasKilled = false;

        if(health <= 0)
        {
            IsDead();
        }

        if (Input.GetKeyUp(KeyCode.E)) blockAction = -1;
        else if (Input.GetKeyDown(KeyCode.E)) blockAction = 1;

        isInFrontOf = IsInFrontOf();
    }

    public override void OnEpisodeBegin()
    {
        // Random spawning
        //randomSpawn.MoveToSafeRandomPosition(xMinValueSpawning, xMaxValueSpawning, zMinValueSpawning, zMaxValueSpawning);
        //transform.localPosition = randomSpawn.localPosition;
        //transform.localRotation = randomSpawn.localRotation;

        transform.localPosition = initalPosition;
        transform.localRotation = initalRotation;

        health = initialHealth;

        stamina.staminaValue = 1.00666f;

        env.AddAgent();

        episodeBegin = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localRotation.normalized);
        sensor.AddObservation(transform.localPosition.normalized);
        sensor.AddObservation(isAttacking);
        sensor.AddObservation(isBlocking);
        sensor.AddObservation(Mathf.Clamp(stamina.staminaValue, 0f, 1f));

        // DONT FORGET OBS FOR THE NPC  
        if(closestCharacter != null)
        {
            if (closestCharacter.GetComponent<FightAgent>() != null)
            {
                FightAgent agent = closestCharacter.GetComponent<FightAgent>();

                Vector3 toClosestEnemy = agent.transform.localPosition - transform.localPosition;

                sensor.AddObservation(toClosestEnemy.normalized);
                sensor.AddObservation(agent.transform.localPosition.normalized);
                sensor.AddObservation(agent.transform.localRotation.normalized);
                sensor.AddObservation(isInFrontOf);
                sensor.AddObservation(agent.isAttacking);
                sensor.AddObservation(agent.isBlocking);
            }
            else if(closestCharacter.GetComponent<NPC_Fighter>() != null)
            {
                NPC_Fighter npc = closestCharacter.GetComponent <NPC_Fighter>();

                Vector3 toClosestEnemy = npc.transform.localPosition - transform.localPosition;

                sensor.AddObservation(toClosestEnemy.normalized);
                sensor.AddObservation(npc.transform.localPosition.normalized);
                sensor.AddObservation(npc.transform.localRotation.normalized);
                sensor.AddObservation(isInFrontOf);
                sensor.AddObservation(npc.isAttacking);
                sensor.AddObservation(false);
            }
        }
        else
        {
            sensor.AddObservation(Vector3.zero.normalized);
            sensor.AddObservation(Vector3.zero.normalized);
            sensor.AddObservation(Quaternion.Euler(0, 0, 0));
            sensor.AddObservation(0f);
            sensor.AddObservation(false);
            sensor.AddObservation(false);
        }
    }

    /// <summary>
    /// ContinuousActions : 
    /// 0 : moveX
    /// 1 : moveZ
    /// 2 : yawRotation
    /// 
    /// DiscreteActions : 
    /// 0 : attack
    /// 1 : block
    /// </summary>
    /// <param name="actions"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float yawRotation = actions.ContinuousActions[2];
        bool attack = actions.ContinuousActions[3] > 0;
        bool block = actions.ContinuousActions[4] > 0;

        // Moving
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        if (movement.magnitude > 0f && canMove) transform.Translate(movement.normalized * Time.deltaTime * movementSpeed, Space.Self);

        //Rotating
        Vector3 rotationVector = transform.rotation.eulerAngles;

        smoothYawRotation = Mathf.MoveTowards(smoothYawRotation, yawRotation, 2f * Time.fixedDeltaTime);

        float yaw = rotationVector.y + smoothYawRotation * Time.fixedDeltaTime * yawRotationSpeed;

        if(canRotate)   transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        //Attacking
        if (attack && canAttack && stamina.attackStaminaCost <= stamina.staminaValue)
        {
            Attack();
            StartCoroutine(AttackDelay());
        }

        //Blocking
        if (stamina.blockStaminaCost <= stamina.staminaValue)
        {
            if (block && canBlock) Block();
            else if (!block && !isAttacking) StopBlocking();
        }
        else StopBlocking();

        // Get the closest character to us
        closestCharacter = env.GetClosestCharacter(env.characters, transform.localPosition);

        if (env.AgentsCount <= 1) EndEpisode();

        if (stamina.staminaValue <= stamina.attackStaminaCost || stamina.staminaValue <= stamina.blockStaminaCost) AddReward(-0.50f);

        //if (closestCharacter != null)
        //{
        //    if (Vector3.Distance(transform.position, closestCharacter.transform.position) <= 1.5f) AddReward(0.25f / MaxStep);
        //}

       // if (isInFrontOf) AddReward(0.25f / MaxStep);

        //if (isInBackOf) AddReward(-0.5f / MaxStep);

        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Moving
        if(canMove)
        {
            continuousActions[0] = ((int)Input.GetAxisRaw("Horizontal"));
            continuousActions[1] = ((int)Input.GetAxisRaw("Vertical"));
        }

        // Rotation
        if (Input.GetKey(KeyCode.B)) continuousActions[2] = -1;
        else if (Input.GetKey(KeyCode.M)) continuousActions[2] = 1;

        // Attack and block
        if (Input.GetKey(KeyCode.R)) continuousActions[3] = 1;

        continuousActions[4] = blockAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwordHit"))
        {
            if(!isBlocking && canBeHit)
            {
                TakingDamage();
                attackArea.gameObject.active = false;
                StartCoroutine(canBeHitDelay());
            }
        }
        else if (other.CompareTag("SwordHit"))
        {
            if(isBlocking)  AddReward(0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bound"))
        {
            env.AgentsCount--;
            AddReward(-6f);
        }
        else if (collision.gameObject.CompareTag("Bottom")) env.AgentsCount--;
    }

    /// <summary>
    /// Setups the variables to animate the agent in the animator
    /// </summary>
    private void Animating()
    {
        // Animating
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);

        animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isBlocking", isBlocking);
    }

    /// <summary>
    /// Called when the agent attacks
    /// </summary>
    private void Attack()
    {
        if(closestCharacter != null) if(Vector3.Distance(transform.position, closestCharacter.transform.position) >= 1.5f)   AddReward(-0.10f);


        //Conditions
        canAttack = false;
        isBlocking = false;
        canBlock = false;
        canMove = false;
        isAttacking = true;
        canRotate = false;
        attackArea.gameObject.active = true;

        // Animating
        animator.Play(name = "Sword And Shield Slash");

        //Stamina
        stamina.staminaValue = stamina.StaminaDeduction(stamina.staminaValue, stamina.attackStaminaCost);
    }

    /// <summary>
    /// Returns every conditions for the attack to be finished after the delay
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.7f);
        yield return canAttack = true;
        yield return isAttacking = false;
        yield return attackArea.gameObject.active = false;
        yield return canMove = true;
        yield return canRotate = true;
        yield return canBlock = true;
    }

    IEnumerator canBeHitDelay()
    {
        yield return new WaitForSeconds(0.7f);
        yield return canBeHit = true;
    }

    /// <summary>
    /// Called when the agent is blocking
    /// </summary>
    private void Block()
    {
        animator.Play(name = "Block");
        isBlocking = true;
        canMove = false;
    }

    /// <summary>
    /// Called when the agent stops blocking or is forced beacause of stamina value
    /// </summary>
    private void StopBlocking()
    {
        isBlocking = false;
        canMove = true;
    }

    /// <summary>
    /// Takes the damage done of the health 
    /// </summary>
    private void TakingDamage()
    {
        if (health <= damage)
        {
            health = 0f;
        }
        else
        {
            AddReward(-0.50f);
            health -= damage;
            canBeHit = false;
        }
    }

    /// <summary>
    /// Called every update, if the agent killed it ends the episode and adds +1 reward
    /// </summary>
    /// <param name="hasKilled">True if the agent killed</param>
    private void HasKilled(bool hasKilled)
    {
        if(hasKilled)
        {
            hasKilled = false;
            env.AgentsCount--;
            AddReward(1f);
        }
    }

    /// <summary>
    /// Called ewvery update, hashit is true when agent has hit his enemy
    /// </summary>
    /// <param name="hasHit"></param>
    private void HasHit(bool hasHit)
    {
        if(hasHit)
        {
            hasHit = false;
            AddReward(0.5f);
        }
    }

    /// <summary>
    /// Is executed when the agent dies, ends episode and adds -1 reward 
    /// </summary>
    private void IsDead()
    {
        AddReward(-2f);
        env.AgentsCount--;
    }

    private bool IsInFrontOf()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);
        Physics.SphereCast(ray, sphereCastRadius, out hit, Mathf.Infinity);

        if(hit.collider != null)
        {
            if (hit.collider.CompareTag("DummyAgent") || hit.collider.CompareTag("BlueAgent") || hit.collider.CompareTag("RedAgent")) return true;
            else return false;
        }

        return false;
    }
}

