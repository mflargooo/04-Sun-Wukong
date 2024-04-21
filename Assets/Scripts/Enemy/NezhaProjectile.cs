using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NezhaProjectile : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem pulse;
    [SerializeField] private Collider pulseCollider;
    [SerializeField] private float timeBTWPulses;
    [SerializeField] private float timeTilMaxPulseDist;
    [SerializeField] private float rotateVelocity;
    [SerializeField] private uint numPulses;
    [SerializeField] private float boomerangSpeed;
    [SerializeField] private float boomerangReturnSpeed;

    private void Update()
    {
        model.transform.Rotate(Vector3.up * rotateVelocity * Time.deltaTime);
    }

    public void BoomerangTo(Transform returnLoc, Vector3 targetPos, int type)
    {
        StartCoroutine(Boomerang(returnLoc, targetPos, type));
    }

    float BoomerangPathRadius(float k, float time)
    {
        return Mathf.Abs(k * Mathf.Sin(time));
    }

    IEnumerator Boomerang(Transform returnLoc, Vector3 targetPos, int type)
    {
        Vector3 startPos = returnLoc.position;
        Vector3 diff = targetPos - startPos;
        float k = diff.magnitude / 2;
        float angle = Mathf.Deg2Rad * Vector3.SignedAngle(Vector3.forward, diff.normalized, Vector3.up);
        float timer = 0;

        while (timer < Mathf.PI / 2)
        {
            transform.position = startPos + new Vector3(Mathf.Cos(timer) - Mathf.Sin(angle), 0f, Mathf.Sin(timer) + Mathf.Cos(angle)) * BoomerangPathRadius(k, timer);
            timer += Time.deltaTime * boomerangSpeed;
            yield return null;
        }
        if (type == 1)
        {
            yield return StartCoroutine(Pulse());
        }
        while (timer < Mathf.PI)
        {
            transform.position = startPos + new Vector3(Mathf.Cos(timer) - Mathf.Sin(angle), 0f, Mathf.Sin(timer) + Mathf.Cos(angle)) * BoomerangPathRadius(k, timer);
            timer += Time.deltaTime * boomerangSpeed;
            yield return null;
        }
        
        while((diff = returnLoc.position - transform.position).magnitude > .1f)
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
            if (!pulse.isPlaying) pulse.Play();
            yield return new WaitForSeconds(timeTilMaxPulseDist);
            pulseCollider.enabled = true;
            yield return new WaitForSeconds(.2f);
            pulseCollider.enabled = false;
            yield return new WaitForSeconds(timeBTWPulses);
        }
    }
}
