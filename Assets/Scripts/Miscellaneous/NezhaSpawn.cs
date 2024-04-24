using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NezhaSpawn : MonoBehaviour
{
    [Header("SHOULD BE A CHILD AND ENABLED IN HIERARCHY")]
    [SerializeField] private GameObject thingToSummon;
    [SerializeField] private MonoBehaviour[] scriptsToEnable;
    [SerializeField] private Collider[] collidersToEnable;

    private void Start()
    {
        thingToSummon.GetComponent<NavMeshAgent>().enabled = false;
        foreach (Collider collider in collidersToEnable)
        {
            collider.enabled = false;
        }
        foreach (MonoBehaviour script in scriptsToEnable)
        {
            script.enabled = false;
        }
    }

    private void SummonThing()
    {
        thingToSummon.transform.parent = null;
        foreach (Collider collider in collidersToEnable)
        {
            collider.enabled = true;
        }
        foreach (MonoBehaviour script in scriptsToEnable)
        {
            script.enabled = true;
        }
        thingToSummon.GetComponent<NavMeshAgent>().enabled = true;
        thingToSummon.GetComponent<Nezha>().StartNezha();
        Destroy(gameObject, 2f);
    }
}
