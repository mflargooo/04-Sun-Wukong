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

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        ftsps = footsteps;
        ftspsSrc = footstepsSource;
    }
    public static void PlayFootstep()
    {
        ftspsSrc.PlayOneShot(ftsps[Random.Range(0, ftsps.Length)]);
    }
}
