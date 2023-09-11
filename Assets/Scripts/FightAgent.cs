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

public class FightAgent : Agent
{
    /// <summary>
    /// TODO : Stamina function (not forget on collect obs) and machine vision
    /// 
    /// Stamina : 
    /// 
    /// Max actions : 
    /// Attacking : 2 times
    /// Blocking : 2 seconds
    /// 
    /// Max stamina = 1.00f
    /// 
    /// Attacking = 0.50f stamina
    /// Blocking = 0.01f every fixed update
    /// 
    /// Gaining rate (note : if stamina == 0, wait 1 sec. and add a negative reward) : 1.5 sec. for full stamina
    /// 
    /// Make function to customize every parameter in engine, to change values dynamically (50 * seconds, stamina \ by the result)
    /// </summary>


    //VARIABLES

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

    // Tags
    [Header("Tags")]
    [SerializeField] private string ennemySwordHitTag;
    [SerializeField] private string allySwordHitTag;
    [Space(15)]

    // Scripts variables
    [Header("Scripts Variables")]
    Animator animator;
    AttackArea attackArea;
    [Space(15)]

    // Combat conditions
    [Header("Combat Conditions")]
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isBlocking;
    bool canAttack = true;
    bool canBlock = true;
    bool canMove = true;
    bool canRotate = true;
    [Space(15)]

    // Rotation
    private float smoothYawRotation = 1f;

    // Stamina
    [Header("Stamina")]
    [SerializeField] float stamina;
    [SerializeField] float StaminaGainingRate;
    [SerializeField] int MaxNumberOfAttacks;
    [SerializeField] int MaxSecondsOfBlock;
    float attackStaminaCost;
    float blockStaminaCost;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackArea = GameObject.Find("AttackArea").GetComponent<AttackArea>();

        attackArea.gameObject.active = false;
    }

    private void Start()
    {
        attackStaminaCost = stamina / MaxNumberOfAttacks;
        // Times 50 because fixed update executes 50 times a second
        blockStaminaCost = stamina / (50 * MaxSecondsOfBlock);
    }

    private void Update()
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

        HasKilled(attackArea.hasKilled);

        attackArea.hasKilled = false;
    }

    /// <summary>
    /// Called every 0.02 seconds (50 times per seconds)
    /// </summary>
    private void FixedUpdate()
    {
        if (isBlocking && blockStaminaCost <= stamina)
        {
            stamina = Stamina(stamina, blockStaminaCost);
        }

        Debug.Log(stamina);
    }

    public override void OnEpisodeBegin()
    {
        // Random spawning
        transform.localPosition = new Vector3(UnityEngine.Random.Range(xMinValueSpawning, xMaxValueSpawning), 0f, UnityEngine.Random.Range(zMinValueSpawning, zMaxValueSpawning));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // TODO : Collect the location of the ennemies and the allies make function to find them in child of the env, rotation of the agent, vector from the agent to the ennemy
        // REMEMBER TO ALWAYS NORMALIZE

        // Observe the local position and rotation of the red agents

        sensor.AddObservation(transform.localRotation.normalized);
        sensor.AddObservation(transform.localPosition.normalized);
        sensor.AddObservation(isAttacking);
        sensor.AddObservation(isBlocking);
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
        // TODO : Add the actions that the agent can do, moveX, moveZ, Attack, Block and rotation
        float moveX = actions.DiscreteActions[0];
        float moveZ = actions.DiscreteActions[1];
        float yawRotation = actions.DiscreteActions[2];
        bool attack = actions.DiscreteActions[3] > 0;
        bool block = actions.DiscreteActions[4] > 0;

        // Moving
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        if (movement.magnitude > 0f && canMove) transform.Translate(movement.normalized * Time.deltaTime * movementSpeed, Space.Self);

        //Rotating
        Vector3 rotationVector = transform.rotation.eulerAngles;

        smoothYawRotation = Mathf.MoveTowards(smoothYawRotation, yawRotation, 2f * Time.fixedDeltaTime);

        float yaw = rotationVector.y + smoothYawRotation * Time.fixedDeltaTime * yawRotationSpeed;

        if(canRotate)   transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        //Attacking
        if (attack && canAttack && attackStaminaCost <= stamina)
        {
            Attack();
            StartCoroutine(AttackDelay());
        }

        //Blocking
        if (blockStaminaCost <= stamina)
        {
            if (block && canBlock) Block();
            else if (!block && !isAttacking) StopBlocking();
        }
        else StopBlocking();


        // Check to change for greater value for motivating the AI to kill more rapidly
        AddReward(-0.1f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //  TODO : Add the actions that the agent can do to test if working
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Moving
        if(canMove)
        {
            discreteActions[0] = ((int)Input.GetAxisRaw("Horizontal"));
            discreteActions[1] = ((int)Input.GetAxisRaw("Vertical"));
        }

        // Rotation
        if (Input.GetKey(KeyCode.B)) discreteActions[2] = -1;
        else if (Input.GetKey(KeyCode.M)) discreteActions[2] = 1;

        // Attack and block
        if (Input.GetKey(KeyCode.R)) discreteActions[3] = 1;
        else if(Input.GetKey(KeyCode.E)) discreteActions[4] = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ennemySwordHitTag))
        {
            if(isBlocking == false)
            {
                IsDead();
            }
        }
        else if (other.CompareTag(ennemySwordHitTag) && isBlocking)
        {
            AddReward(0.8f);
        }

        if (other.CompareTag(allySwordHitTag)) AddReward(-0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bound"))
        {

            AddReward(-1f);
        }
        else if (collision.gameObject.CompareTag("Bottom")) EndEpisode();
    }

    /// <summary>
    /// Called when the agent attacks
    /// </summary>
    private void Attack()
    {
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
        stamina = Stamina(stamina, attackStaminaCost);
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
    /// Called every update, if the agent killed it ends the episode and adds +1 reward
    /// </summary>
    /// <param name="hasKilled">True if the agent killed</param>
    private void HasKilled(bool hasKilled)
    {
        if(hasKilled)
        {
            hasKilled = false;
            AddReward(1f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Is executed when the agent dies, ends episode and adds -1 reward 
    /// </summary>
    private void IsDead()
    {
        AddReward(-1f);
        
        EndEpisode();
    }

    /// <summary>
    /// Returns the correct amount of stamina after an action
    /// </summary>
    /// <returns></returns>
    private float Stamina(float functionStamina, float staminaCost)
    {    
        return functionStamina -= staminaCost;
    }
}

