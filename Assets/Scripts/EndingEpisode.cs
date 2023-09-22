using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingEpisode : MonoBehaviour
{

    [HideInInspector] public float AgentsCount;

    // Update is called once per frame
    void Update()
    {
        if (AgentsCount <= 1)   AgentsCount = 0;
    }

    public void AddAgent()
    {
        AgentsCount++;
    }
}
