using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Fighter : MonoBehaviour
{

    private FightAgent fightAgent;
    private Env env;

    private void Awake()
    {
        fightAgent = GetComponent<FightAgent>();
        env = GetComponent<Env>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Put GetToClosestAgent in the fixed update
    private void FixedUpdate()
    {
        transform.localPosition = GetToClosestAgent();
    }

    private Vector3 GetToClosestAgent()
    {
        FightAgent closestAgent = env.GetClosestAgent(env.fightAgents, transform.localPosition);

        return Vector3.MoveTowards(transform.localPosition, closestAgent.transform.localPosition, 1);
    }
}
