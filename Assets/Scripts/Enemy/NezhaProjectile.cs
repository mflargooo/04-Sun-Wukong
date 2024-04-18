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

    Vector2 GetEllipseFromPoles(Vector2 p1, Vector2 p2)
    {
        Vector2 sum = p2 + p1;
        Vector2 midpoint = sum / 2;
        float b2 = (p1 - midpoint).sqrMagnitude;
    }
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
