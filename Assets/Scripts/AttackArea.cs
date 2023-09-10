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


    private FightAgent fightAgent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<FightAgent>() != null)
        {
            fightAgent = other.transform.gameObject.GetComponent<FightAgent>();
        }


        if (other.CompareTag(ennemyTag) && fightAgent.isBlocking == false)
        {
            hasKilled = true;
        }
    }
}
