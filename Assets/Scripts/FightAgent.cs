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
    /// TODO : Stamina function and machine vision
    /// </summary>

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float yawRotationSpeed = 3f;
    [SerializeField] float zMaxValueSpawning;
    [SerializeField] float zMinValueSpawning;
    [SerializeField] float xMaxValueSpawning;
    [SerializeField] float xMinValueSpawning;

    [SerializeField] private string ennemySwordHitTag;
    [SerializeField] private string allySwordHitTag;

    Animator animator;
    AttackArea attackArea;

    [HideInInspector] public bool isAttacking;
    bool canAttack = true;
    [HideInInspector] public bool isBlocking;
    bool canBlock = true;
    bool canMove = true;
    bool canRotate = true;

    private float smoothYawRotation = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackArea = GameObject.Find("AttackArea").GetComponent<AttackArea>();

        attackArea.gameObject.active = false;
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

        sensor.AddObservation(transform.localRotation.normalized);  // 4 observations
        sensor.AddObservation(transform.localPosition.normalized);  // 3 observations
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
        if (attack && canAttack)
        {
            Attack();
            StartCoroutine(AttackDelay());
        }

        //Blocking
        if (block && canBlock)
        {
            Block();
        }
        else if(!block && !isAttacking)
        {
            isBlocking = false;
            canMove = true;
        }

        if (isAttacking && !attackArea.hasKilled)    AddReward(-0.5f);

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
            //Get the ennemy and end episode too
            AddReward(-1f);
        }
        else if (collision.gameObject.CompareTag("Bottom")) EndEpisode();
    }

    private void Attack()
    {
        //Animating
        canAttack = false;
        isBlocking = false;
        canBlock = false;
        canMove = false;
        isAttacking = true;
        canRotate = false;
        attackArea.gameObject.active = true;
        animator.Play(name = "Sword And Shield Slash");
    }

    private void Block()
    {
        animator.Play(name = "Block");
        isBlocking = true;
        canMove = false;
    }

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

    private void HasKilled(bool hasKilled)
    {
        if(hasKilled)
        {
            hasKilled = false;
            AddReward(1f);
            EndEpisode();
        }
    }

    private void IsDead()
    {
        AddReward(-1f);
        
        EndEpisode();
    }
}

