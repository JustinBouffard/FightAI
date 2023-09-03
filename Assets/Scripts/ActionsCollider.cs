using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsCollider : MonoBehaviour
{
    [SerializeField] FightAgent fightAgent;

    private void Awake()
    {
        gameObject.tag = "Idle";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fightAgent != null)
        {
            if (!fightAgent.isBlocking && !fightAgent.isAttacking)
            {
                gameObject.tag = "Idle";
            }
            else if (fightAgent.isBlocking)
            {
                gameObject.tag = "IsBlocking";
            }
            else if (fightAgent.isAttacking)
            {
                gameObject.tag = "IsAttacking";
            }
        }

    }
}
