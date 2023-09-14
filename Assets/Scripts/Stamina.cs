using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    // Stamina
    [Header("Stamina")]
    [SerializeField] float stamina;
    [SerializeField] float MaxSecondsGain;
    [SerializeField] float WaitSecondsStaminaGain;
    [SerializeField] int MaxNumberOfAttacks;
    [SerializeField] int MaxSecondsOfBlock;
    float initialStamina;
    float gainingStamina;
    float gainingStaminaDelay;
    bool canGainStamina = false;
    float attackStaminaCost;
    float blockStaminaCost;
    float staminaDelay;

    FightAgent fightAgent;


    private void Awake()
    {
        fightAgent = GetComponent<FightAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stamina cost while blocking
        if (fightAgent.isBlocking && blockStaminaCost <= stamina) stamina = StaminaDeduction(stamina, blockStaminaCost);

        StaminaReplenish();

        Debug.Log(staminaDelay);
    }

    /// <summary>
    /// Returns the correct amount of stamina after an action
    /// </summary>
    /// <returns></returns>
    private float StaminaDeduction(float functionStamina, float staminaCost)
    {
        return functionStamina -= staminaCost;
    }

    /// <summary>
    /// Returns the correct amount of stamina while idle
    /// </summary>
    /// <param name="functionStamina"></param>
    /// <param name="staminaCost"></param>
    /// <returns></returns>
    private float StaminaGain(float functionStamina, float gainingStamina)
    {
        return functionStamina += gainingStamina;
    }


    /// <summary>
    /// Replenishes the stamina after action with a delay
    /// </summary>
    private void StaminaReplenish()
    {
        // TODO : Make function or move it to a different script
        if (!fightAgent.isBlocking && !fightAgent.isAttacking && staminaDelay <= WaitSecondsStaminaGain && !canGainStamina)
        {
            staminaDelay = StaminaGain(staminaDelay, gainingStaminaDelay);
        }
        else if (staminaDelay >= WaitSecondsStaminaGain)
        {
            canGainStamina = true;
            staminaDelay = 0f;
        }

        //Check if the actor attacks or blocks after the delay, so it can happen again
        if (fightAgent.isAttacking || fightAgent.isBlocking) canGainStamina = false;

        if (canGainStamina && stamina <= initialStamina)
        {
            stamina = StaminaGain(stamina, gainingStamina);
        }
        else canGainStamina = false;
    }
}
