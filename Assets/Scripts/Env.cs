using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Env : MonoBehaviour
{

    [HideInInspector] public float AgentsCount;
    [SerializeField] public List<GameObject> characters;

    private void Awake()
    {
        characters = new List<GameObject>();
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
                characters.Add(child.gameObject);

                FindAgents(child);
            }
            else FindAgents(child);
        }
    }

    public GameObject GetClosestCharacter(List<GameObject> characters, Vector3 selfLocalPosition)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach(GameObject potentialTarget in characters)
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
