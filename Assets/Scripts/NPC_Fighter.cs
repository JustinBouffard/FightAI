using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Fighter : MonoBehaviour
{

    private FightAgent fightAgent; 

    // Start is called before the first frame update
    void Start()
    {
        fightAgent = GetComponent<FightAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
