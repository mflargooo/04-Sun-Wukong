using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioSource footstepsSource;

    [SerializeField] private AudioClip[] playerSwings;
    [SerializeField] private AudioSource playerSwingsSource;

    [SerializeField] private AudioClip[] connectedAttacks;
    [SerializeField] private AudioSource connectedAttackSource;

    [SerializeField] private AudioClip[] meleeSwoosh;
    [SerializeField] private AudioSource meleeSwooshSource;

    [SerializeField] private AudioClip gong;
    [SerializeField] private AudioSource gongSource;

    [SerializeField] private AudioClip cheer;
    [SerializeField] private AudioSource cheerSource;

    [SerializeField] private AudioClip healthPickup;
    [SerializeField] private AudioSource healthPickupSource;

    [SerializeField] private AudioClip metalSwooshSound;
    [SerializeField] private AudioSource metalSwooshSource;

    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioSource throwSource;

    [SerializeField] private AudioClip[] bowPullbackSound;
    [SerializeField] private AudioSource bowPullbackSource;

    [SerializeField] private AudioClip[] bowReleaseSound;
    [SerializeField] private AudioSource bowReleaseSource;

    [SerializeField] private AudioClip pulseSound;
    [SerializeField] private AudioSource pulseSource;

    [SerializeField] private AudioClip spinningWhooshSound;
    [SerializeField] private AudioSource spinningWhooshSource;

    [SerializeField] private AudioClip nezhaLaughSound;
    [SerializeField] private AudioSource nezhaLaughSource;

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }
    public void PlayFootstep()
    {
        footstepsSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
    }

    public void PlayPlayerSwing()
    {
        playerSwingsSource.PlayOneShot(playerSwings[Random.Range(0, playerSwings.Length)]);
    }
    public void PlayConnectedAttack()
    {
        connectedAttackSource.PlayOneShot(connectedAttacks[Random.Range(0, connectedAttacks.Length)]);
    }

    public void PlayMeleeSwoosh()
    {
        meleeSwooshSource.PlayOneShot(meleeSwoosh[Random.Range(0, meleeSwoosh.Length)]);
    }

    public void PlayGong()
    {
        gongSource.PlayOneShot(gong);
    }
    public void PlayCheer()
    {
        cheerSource.PlayOneShot(cheer);
    }
    public void PlayHealthPickup()
    {
        healthPickupSource.PlayOneShot(healthPickup);
    }

    public void PlayMetalSwoosh()
    {
        metalSwooshSource.PlayOneShot(metalSwooshSound);
    }
    public void PlayThrowSound()
    {
        throwSource.PlayOneShot(throwSound);
    }
    public void PlayBowPullbackSound()
    {
        bowPullbackSource.PlayOneShot(bowPullbackSound[Random.Range(0, bowPullbackSound.Length)]);
    }

    public void PlayBowReleaseSound()
    {
        bowReleaseSource.PlayOneShot(bowReleaseSound[Random.Range(0, bowReleaseSound.Length)]);
    }

    public void PlayPulseSound()
    {
        pulseSource.PlayOneShot(pulseSound);
    }

    public void PlaySpinningWhooshSound()
    {
        spinningWhooshSource.PlayOneShot(spinningWhooshSound);
    }
    public void PlayNezhaLaugh()
    {
        nezhaLaughSource.PlayOneShot(nezhaLaughSound);
    }
}
