using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatToText : MonoBehaviour
{
    [SerializeField] private Stamina stamina;
    [SerializeField] private FightAgent fightAgent;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI healthText;
    float healthValue;
    float staminaValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthValue = fightAgent.health;
        healthText.text = healthValue.ToString();

        staminaValue = stamina.staminaValue;
        staminaText.text = staminaValue.ToString();
    }
}
