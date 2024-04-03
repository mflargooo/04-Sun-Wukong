using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private SoundManager instance;

    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioSource footstepsSource;
    private static AudioClip[] ftsps;
    private static AudioSource ftspsSrc;

    [SerializeField] private AudioClip[] playerHits;
    [SerializeField] private AudioSource playerHitsSource;
    private static AudioClip[] phs;
    private static AudioSource phsSrc;

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        ftsps = footsteps;
        ftspsSrc = footstepsSource;
        phs = playerHits;
        phsSrc = playerHitsSource;
    }
    public static void PlayFootstep()
    {
        ftspsSrc.PlayOneShot(ftsps[Random.Range(0, ftsps.Length)]);
    }

    public static void PlayPlayerHit()
    {
        phsSrc.PlayOneShot(phs[Random.Range(0, phs.Length)]);
    }
}
