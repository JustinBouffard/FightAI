using System.Collections;
using System.Collections.Generic;
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

            if (child.CompareTag("RedAgent"))
            {
                fightAgents.Add(child.GetComponent<FightAgent>());

                FindAgents(child);
            }
            else if (child.CompareTag("BlueAgent"))
            {
                fightAgents.Add(child.GetComponent<FightAgent>());

                FindAgents(child);
            }
            else if (child.CompareTag("DummyAgent"))
            {
                fightAgents.Add(child.GetComponent<FightAgent>());

                FindAgents(child);
            }
            else FindAgents(child);
        }
    }
}
