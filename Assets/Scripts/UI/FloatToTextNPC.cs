using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatToTextNPC : MonoBehaviour
{
    [SerializeField] private NPC_Fighter npc;
    [SerializeField] private TextMeshProUGUI healthText;
    float healthValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthValue = npc.health;
        healthText.text = healthValue.ToString();
    }
}
