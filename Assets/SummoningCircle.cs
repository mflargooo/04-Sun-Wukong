using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummoningCircle : MonoBehaviour
{
    [SerializeField] private ParticleSystem doSummoningParticles;
    [SerializeField] private ParticleSystem finishSummoningParticles;
    [SerializeField] private GameObject summonScaling;

    [Header("SHOULD BE A CHILD AND ENABLED IN HIERARCHY")]
    [SerializeField] private GameObject thingToSummon;
    [SerializeField] private Component[] scriptsToEnable;

    private void Awake()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0f, 360f), transform.eulerAngles.z);
        foreach (MonoBehaviour script in scriptsToEnable)
        {
            script.enabled = false;
        }
    }

    private void PlayDoSummoningParticles()
    {
        if (!doSummoningParticles.isPlaying)
            doSummoningParticles.Play();
    }

    private void PlayFinishSummoningParticles()
    {
        if (!finishSummoningParticles.isPlaying)
            finishSummoningParticles.Play();
    }

    private void SummonThing()
    {
        thingToSummon.transform.parent = null;
        foreach (MonoBehaviour script in scriptsToEnable)
        {
            script.enabled = true;
        }
        Destroy(gameObject, 2f);
    }
}
