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
            if(character.CompareTag("BlueAgent"))
            {
                if(!character.GetComponent<FightAgent>().isBlocking)
                {
                    if (other.GetComponent<FightAgent>().health <= other.GetComponent<FightAgent>().damage) hasKilled = true;
                    else hasHit = true;
                }
            }
        }
        else if (character.GetComponent<NPC_Fighter>() != null)
        {
            if (character.CompareTag("DummyAgent"))
            {
                if(!character.GetComponent<NPC_Fighter>().isProtected)
                {
                    if (other.GetComponent<NPC_Fighter>().health <= other.GetComponent<NPC_Fighter>().damage) hasKilled = true;
                    else hasHit = true;
                }
            }
        }
    }
}
