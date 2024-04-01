using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricPlayerController3D : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float modelRotateMultiplier;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    private float dashCD;

    public Vector3 input { get; private set; }
    public Vector3 isometricInput { get; private set; }
    private Vector3 lastFacing;

    private float inputRotationRad = Mathf.PI / 4;
    private float cosInputRot, sinInputRot;
    float isoLookAngle;
    Vector3 isoNorm;


    private Coroutine state;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cosInputRot = Mathf.Cos(inputRotationRad);
        sinInputRot = Mathf.Sin(inputRotationRad);

        lastFacing = new Vector3(-sinInputRot, 0f, cosInputRot);
        isoLookAngle = 0;
        isoNorm = Vector3.forward;

        state = StartCoroutine(Movement());

    }
    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        isometricInput = new Vector3(input.x * cosInputRot - input.z * sinInputRot, 0f, input.z * cosInputRot + input.x * sinInputRot);

        if(dashCD > 0)
        {
            dashCD -= Time.deltaTime;
        }
    }

    IEnumerator Movement()
    {
        while (true)
        {
            isoNorm = isometricInput.normalized;
            rb.velocity = isoNorm * moveSpeed + rb.velocity.y * Vector3.up;

            if(isometricInput.sqrMagnitude > 0)
                lastFacing = isoNorm.normalized;
            else 
                isoLookAngle = Vector3.SignedAngle(modelTransform.forward, isoNorm, Vector3.up);

            if (Mathf.Abs(isoLookAngle) > 2f) modelTransform.Rotate(Vector3.up, isoLookAngle * modelRotateMultiplier * Time.deltaTime);
            else modelTransform.rotation = Quaternion.LookRotation(lastFacing, Vector3.up);

            if(Input.GetKeyDown(KeyCode.LeftShift) && dashCD <= 0f)
            {
                NextState(Dash());
            }

            yield return null;
        }
    }

    IEnumerator Dash()
    {
        float dt = dashTime;
        while (dt > 0)
        {
            rb.velocity = lastFacing.normalized * dashSpeed;
            dt -= Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector3.zero;
        dashCD = dashCooldown;
        NextState(Movement());
    }

    void NextState(IEnumerator next)
    {
        StopCoroutine(state);
        state = StartCoroutine(next);
    }
}
