using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NezhaProjectile : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem pulse;
    [SerializeField] private float timeBTWPulses;
    [SerializeField] private float rotateVelocity;
    [SerializeField] private uint numPulses;
    [SerializeField] private float boomerangSpeed;
    [SerializeField] private float boomerangReturnSpeed;

    private void Update()
    {
        model.transform.Rotate(Vector3.up * rotateVelocity * Time.deltaTime);
        model.transform.Rotate(Vector3.right * rotateVelocity * Time.deltaTime);
        model.transform.Rotate(Vector3.forward * rotateVelocity * Time.deltaTime);
    }

    public void BoomerangTo(Transform returnLoc, Vector3 targetPos)
    {
        StartCoroutine(Boomerang(returnLoc, targetPos));
    }

    IEnumerator Boomerang(Transform returnLoc, Vector3 targetPos)
    {
        Vector3 startPos = returnLoc.position;
        float  timer = 0;
        while (timer < 1 / boomerangSpeed)
        {
            transform.position = Vector3.Slerp(startPos, targetPos, timer * boomerangSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(Pulse());
        
        Vector3 diff = Vector3.zero;
        while ((diff = returnLoc.position - transform.position).sqrMagnitude > .25f)
        {
            transform.position += diff.normalized * boomerangReturnSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator Pulse()
    {
        for (uint i = 0; i < numPulses; i++)
        {
            yield return new WaitForSeconds(timeBTWPulses);
            if (!pulse.isPlaying) pulse.Play();
            SoundManager.instance.PlayPulseSound();
        }

        yield return new WaitForSeconds(.5f);
    }
}
