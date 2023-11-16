using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Env : MonoBehaviour
{

    [HideInInspector] public float AgentsCount;
    [SerializeField] public List<FightAgent> fightAgents;

    private void Awake()
    {
        fightAgents = new List<FightAgent>();
    }

    private void Start()
    {
        FindAgents(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (AgentsCount <= 1) AgentsCount = 0;
    }

    public void AddAgent()
    {
        AgentsCount++;
    }

    public void FindAgents(Transform parent)
    {
        //Check for dummy agent too
        for(int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("RedAgent") || child.CompareTag("BlueAgent"))
            {
                fightAgents.Add(child.GetComponent<FightAgent>());
            }
            else FindAgents(child);
        }
    }

    public FightAgent GetClosestAgent(List<FightAgent> fightAgents, Vector3 selfLocalPosition)
    {
        FightAgent bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach(FightAgent potentialTarget in fightAgents)
        {
            Vector3 directionToTarget = potentialTarget.transform.localPosition - selfLocalPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if(dSqrToTarget < closestDistanceSqr && dSqrToTarget != 0)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
     
        return bestTarget;
    }
}
