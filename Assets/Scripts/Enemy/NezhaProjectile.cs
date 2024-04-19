using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NezhaProjectile : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject pulse;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip pulseAnim;
    [SerializeField] private float rotateVelocity;
    [SerializeField] private uint numPulses;
    [SerializeField] private float boomerangSpeed;

    private void Update()
    {
        model.transform.Rotate(Vector3.up * rotateVelocity * Time.deltaTime);
    }

    public void BoomerangTo(Transform returnLoc, Vector3 targetPos, int type)
    {
        StartCoroutine(Boomerang(returnLoc, targetPos, type));
    }

    IEnumerator Boomerang(Transform returnLoc, Vector3 targetPos, int type)
    {
        Vector3 diff = Vector3.zero;
        while ((diff = targetPos - transform.position).sqrMagnitude > .01f)
        {
            transform.position += diff.normalized * boomerangSpeed * Time.deltaTime;
            yield return null;
        }
        if (type == 1)
        {
            yield return StartCoroutine(Pulse());
        }
        while((diff = returnLoc.position - transform.position).sqrMagnitude > .01f)
        {
            transform.position += diff.normalized * boomerangSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator Pulse()
    {
        for (uint i = 0; i < numPulses; i++)
        {
            anim.Play("pulse");
            yield return new WaitForSeconds(pulseAnim.length * 1.5f);
        }
    }
}
