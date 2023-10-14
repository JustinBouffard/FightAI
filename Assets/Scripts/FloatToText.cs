using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatToText : MonoBehaviour
{
    [SerializeField] private Stamina stamina;
    [SerializeField] private TextMeshProUGUI text;
    float staminaValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        staminaValue = stamina.staminaValue;
        text.text = staminaValue.ToString();
    }
}
