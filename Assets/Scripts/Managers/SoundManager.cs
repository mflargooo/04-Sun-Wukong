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

    [SerializeField] private AudioClip[] playerSwings;
    [SerializeField] private AudioSource playerSwingsSource;
    private static AudioClip[] pss;
    private static AudioSource pssSrc;

    [SerializeField] private AudioClip[] connectedAttacks;
    [SerializeField] private AudioSource connectedAttackSource;
    private static AudioClip[] cas;
    private static AudioSource caSrc;

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        ftsps = footsteps;
        ftspsSrc = footstepsSource;
        pss = playerSwings;
        pssSrc = playerSwingsSource;
        cas = connectedAttacks;
        caSrc = connectedAttackSource;
    }
    public static void PlayFootstep()
    {
        ftspsSrc.PlayOneShot(ftsps[Random.Range(0, ftsps.Length)]);
    }

    public static void PlayPlayerSwing()
    {
        pssSrc.PlayOneShot(pss[Random.Range(0, pss.Length)]);
    }
    public static void PlayConnectedAttack()
    {
        caSrc.PlayOneShot(cas[Random.Range(0, cas.Length)]);
    }
}
