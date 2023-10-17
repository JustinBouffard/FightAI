using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private string ennemyTag;

    /// <summary>
    /// True when the agent killed an ennemy
    /// </summary>
    [HideInInspector] public bool hasKilled = false;
    [HideInInspector] public bool hasHit = false;


    private FightAgent fightAgent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<FightAgent>() != null)
        {
            fightAgent = other.transform.gameObject.GetComponent<FightAgent>();
        }


        if (other.CompareTag(ennemyTag) && fightAgent.isBlocking == false)
        {
            if (fightAgent.health <= 0f) hasKilled = true;
            else hasHit = true;
        }
    }
}
