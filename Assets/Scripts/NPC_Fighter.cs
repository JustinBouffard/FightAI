using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Fighter : MonoBehaviour
{

    private FightAgent fightAgent;
    private Env env;

    [SerializeField] private FightAgent closestAgent;

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
        GetToClosestAgent();
    }

    private void GetToClosestAgent()
    {
        closestAgent = env.GetClosestAgent(env.fightAgents, transform.localPosition);

        if (closestAgent != null) Debug.Log(closestAgent.name);
    }
}
