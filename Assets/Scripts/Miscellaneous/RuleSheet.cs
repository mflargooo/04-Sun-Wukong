using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSheet : MonoBehaviour
{
    [SerializeField] private Animator blackoutAnim;
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            blackoutAnim.Play("fade_out");
        }
    }
}
