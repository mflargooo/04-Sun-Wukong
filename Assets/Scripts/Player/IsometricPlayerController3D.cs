using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricPlayerController3D : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator anim;
    private Rigidbody rb;

    [Header("Controls")]
    [SerializeField] private PlayerInputs keybinds;

    [Header("Model")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float modelRotateMultiplier;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float dashCD;

    [Header("Attack")]
    [SerializeField] private float attackMovePercent;
    [SerializeField] private AnimationClip[] attacks;
    private const int MAX_COMBO = 3;
    private int attackType = 0;
    

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

            if (isometricInput != Vector3.zero)
            {
                lastFacing = isoNorm;
                isoLookAngle = Vector3.SignedAngle(modelTransform.forward, lastFacing, Vector3.up);

                if (Mathf.Abs(isoLookAngle) > 2f) modelTransform.Rotate(modelTransform.up, isoLookAngle * modelRotateMultiplier * Time.deltaTime);
                else modelTransform.rotation = Quaternion.LookRotation(lastFacing, modelTransform.up);
            }

            if(Input.GetKeyDown(keybinds.dash) && dashCD <= 0f)
            {
                NextState(Dash());
            }
            
            if (Input.GetKeyDown(keybinds.attack))
            {
                NextState(Attack(0));
            }

            yield return null;
        }
    }

    IEnumerator Dash()
    {
        float dt = dashTime;
        modelTransform.rotation = Quaternion.LookRotation(lastFacing, modelTransform.up);
        while (dt > 0)
        {
            if (Input.GetKeyDown(keybinds.attack))
            {
                NextState(Attack(0));
            }

            rb.velocity = lastFacing.normalized * dashSpeed;
            dt -= Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector3.zero;
        dashCD = dashCooldown;
        NextState(Movement());
    }

    IEnumerator Attack(int combo)
    {
        Debug.Log(combo);
        rb.velocity = Vector3.zero;

        bool nextCombo = false;
        bool smoothRestart = false;

        float attackTime = attacks[combo].length;
        float refAttackTime = attacks[combo].length;

        anim.SetInteger("AttackType", combo);

        while (attackTime > 0)
        {
            switch (combo)
            {
                case 0:
                    rb.velocity = isometricInput.normalized * moveSpeed * attackMovePercent;
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }

            if (Input.GetKeyDown(keybinds.attack) && combo < MAX_COMBO - 1)
            {

                if (!nextCombo && attackTime <= refAttackTime * .1f)
                {
                    smoothRestart = true;
                }
                else if (attackTime <= refAttackTime * .75f)
                {
                    nextCombo = true;
                }
            }

            attackTime -= Time.deltaTime;
            yield return null;
        }
            

        if (nextCombo)
        {
            NextState(Attack(combo + 1));
        }
        else if (smoothRestart)
        {
            NextState(Attack(0));
        }
        else
        {
            anim.SetInteger("AttackType", -1);
            NextState(Movement());
        }

        yield return null;
    }

    void NextState(IEnumerator next)
    {
        StopCoroutine(state);
        state = StartCoroutine(next);
    }
}

