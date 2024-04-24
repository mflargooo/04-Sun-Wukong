using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserAnimationEvents : MonoBehaviour
{
    [SerializeField] private ParticleSystem chargeParts;
    [SerializeField] private ParticleSystem beamParts;

    public void DoChargeParticles()
    {
        if (!chargeParts.isPlaying) chargeParts.Play();
    }

    public void DoBeamParticles()
    {
        if (!beamParts.isPlaying) beamParts.Play();
    }

    public void KillGeyser()
    {
        Destroy(gameObject, 1f);
    }
}
