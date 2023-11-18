using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    /// <summary>
    /// True when the agent killed an ennemy
    /// </summary>
    [HideInInspector] public bool hasKilled = false;
    [HideInInspector] public bool hasHit = false;


    private GameObject character;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null)
        {
            character = other.gameObject;
        }

        if(character.GetComponent<FightAgent>() != null)
        {
            if(character.CompareTag("SwordHit") && !character.GetComponent<FightAgent>().isBlocking)
            {
                if (other.GetComponent<FightAgent>().health <= other.GetComponent<FightAgent>().damage) hasKilled = true;
                else hasHit = true;
            }
        }
    }
}
