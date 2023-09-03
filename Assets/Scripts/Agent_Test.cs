using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Test : MonoBehaviour
{
    [HideInInspector] public float movementSpeed = 5f;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isBlocking;
    private bool canMove = true;

    [SerializeField] Collider AttackArea;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        AttackArea.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Block();

        // Reading the input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        //Moving
        if(movement.magnitude > 0f && canMove)
        {
            transform.Translate(movement.normalized * movementSpeed * Time.deltaTime);
        }

        // Animating
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);

        animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isBlocking", isBlocking);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("SwordHit") && !isBlocking)
        {
            Destroy(transform.gameObject);
        }
    }

    private void Attack()
    {
        //Animating
        if (Input.GetMouseButtonDown(0))
        {
            canMove = false;
            isAttacking = true;
            AttackArea.enabled = true;
            animator.Play(name = "Sword And Shield Slash");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(AttackDelay());
            isAttacking=false;
        }
    }

    private void Block()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.Play(name = "Block");
            isBlocking = true;
            canMove = false;
            Debug.Log(canMove);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isBlocking = false;
            canMove = true;
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.7f);
        yield return AttackArea.enabled = false;
        yield return canMove = true;
    }
}
