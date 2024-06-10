using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : ASingleBehaviour<NavigationManager>
{
    List<NavMeshAgent> pool_Agents;
    [SerializeField] GameObject prefab;

    private void Start()
    {
        pool_Agents = new List<NavMeshAgent>(3);
        var agentInstance = Instantiate(prefab).GetComponent<NavMeshAgent>();
        agentInstance.gameObject.SetActive(false);
        pool_Agents.Add(agentInstance);
    }

    public NavMeshAgent GetNavMeshAgent(Vector3 location)
    {
        NavMeshAgent request;

        if(pool_Agents.Count > 0)
        {
            request = pool_Agents[0];
            pool_Agents.RemoveAt(0);
        }
        else
        {
            request = Instantiate(prefab).GetComponent<NavMeshAgent>();
        }

        request.transform.position = location;
        request.gameObject.SetActive(true);
        return request;
    }

    public void Release(NavMeshAgent navMeshAgent)
    {
        if(pool_Agents.Count + 1 > pool_Agents.Capacity)
        {
            Destroy(navMeshAgent.gameObject);
            return;
        }

        navMeshAgent.gameObject.SetActive(false);
        pool_Agents.Add(navMeshAgent);
    }
    
}
