using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateTowards : MonoBehaviour
{
    Rigidbody rb;
    Transform target;

    private float angleToTarget;
    private float turnDirection;
    private float incrementalCos;
    private float incrementalSin;

    [SerializeField] private float angularSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    void Update()
    {
        angleToTarget = Vector3.SignedAngle(rb.velocity, target.transform.position - transform.position, Vector3.up);
        turnDirection = angleToTarget == 0 ? 0 : angleToTarget / Mathf.Abs(angleToTarget);

        incrementalCos = Mathf.Cos(Mathf.Deg2Rad * turnDirection * angularSpeed * Time.deltaTime);
        incrementalSin = Mathf.Sin(Mathf.Deg2Rad * turnDirection * angularSpeed * Time.deltaTime);

        rb.velocity = new Vector3(rb.velocity.x * incrementalCos + rb.velocity.z * incrementalSin, rb.velocity.y, rb.velocity.z * incrementalCos - rb.velocity.x * incrementalSin);
        transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }
}
