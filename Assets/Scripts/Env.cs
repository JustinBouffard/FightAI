using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Env : MonoBehaviour
{

    [HideInInspector] public float AgentsCount;
    [SerializeField] public List<FightAgent> fightAgents;
    [SerializeField] public List<Vector3> agentsDist;

    private void Awake()
    {
        fightAgents = new List<FightAgent>();
        agentsDist = new List<Vector3>();
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

            if (child.CompareTag("RedAgent") || child.CompareTag("BlueAgent") || child.CompareTag("DummyAgent"))
            {
                fightAgents.Add(child.GetComponent<FightAgent>());

                FindAgents(child);
            }
            else FindAgents(child);
        }
    }

    public FightAgent FindClosestAgent()
    {
        Vector3 smallesDist = agentsDist.Min();
        for (int i = 0; i < fightAgents.Count; i++)
        {
            if (fightAgents[i].transform.localPosition == smallesDist)
            {
                return fightAgents[i];
            }
        }

        return fightAgents[0];
    }

    public void FindAgentsDistances(Transform self)
    {
        //Add other agents info when close
        for (int i = 0; i < fightAgents.Count; i++)
        {
           // if (AbsOfVectorSubstraction(self.localPosition, fightAgents[i].transform.localPosition) != new Vector3(0f, 0f, 0f))
            //{
                //Find the absolute value of the operation do a function to calculate
                agentsDist.Add(AbsOfVectorSubstraction(self.localPosition, fightAgents[i].transform.localPosition));
           // }
        }
    }

    private Vector3 AbsOfVectorSubstraction(Vector3 first, Vector3 second)
    {
        return new Vector3(Mathf.Abs(first.x - second.x), Mathf.Abs(first.y - second.y), Mathf.Abs(first.z - second.z));
    }
}
