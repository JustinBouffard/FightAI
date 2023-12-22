using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    // Stamina
    [Header("Stamina")]
    [HideInInspector] public float staminaValue = 1.00666f;
    [SerializeField] float MaxSecondsGain;
    [SerializeField] float WaitSecondsStaminaGain;
    [SerializeField] public int MaxNumberOfAttacks;
    [SerializeField] public int MaxSecondsOfBlock;

    float initialStamina;
    float gainingStamina;
    float gainingStaminaDelay;
    bool canGainStamina = false;
    float staminaDelay;

    [HideInInspector]   public float attackStaminaCost;
    [HideInInspector]   public float blockStaminaCost;

    FightAgent fightAgent;


    private void Awake()
    {
        fightAgent = GetComponent<FightAgent>();
    }

    private void Start()
    {
        attackStaminaCost = staminaValue / MaxNumberOfAttacks;

        // Times 50 because fixed update executes 50 times a second
        blockStaminaCost = staminaValue / (50 * MaxSecondsOfBlock);

        gainingStamina = staminaValue / (50 * MaxSecondsGain);

        gainingStaminaDelay = staminaValue / (50 * WaitSecondsStaminaGain);

        // For the gaining stamina (stamina can't be more than initial stamina)
        initialStamina = staminaValue;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stamina cost while blocking
        if (fightAgent.isBlocking && blockStaminaCost <= staminaValue) staminaValue = StaminaDeduction(staminaValue, blockStaminaCost);

        StaminaReplenish();
    }

    /// <summary>
    /// Returns the correct amount of stamina after an action
    /// </summary>
    /// <returns></returns>
    public float StaminaDeduction(float functionStamina, float staminaCost)
    {
        return functionStamina -= staminaCost;
    }

    /// <summary>
    /// Returns the correct amount of stamina while idle
    /// </summary>
    /// <param name="functionStamina"></param>
    /// <param name="staminaCost"></param>
    /// <returns></returns>
    public float StaminaGain(float functionStamina, float gainingStamina)
    {
        return functionStamina += gainingStamina;
    }


    /// <summary>
    /// Replenishes the stamina after action with a delay
    /// </summary>
    public void StaminaReplenish()
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

        if (canGainStamina && staminaValue <= initialStamina)
        {
            staminaValue = StaminaGain(staminaValue, gainingStamina);
        }
        else canGainStamina = false;
    }
}
