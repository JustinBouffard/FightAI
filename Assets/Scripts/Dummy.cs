using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField] bool isBlocking = false;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlocking) animator.Play(name = "Block");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BlueSwordHit") && !isBlocking)
        {
            Destroy(transform.gameObject);
            Debug.Log("Dead");
        }
    }
}
