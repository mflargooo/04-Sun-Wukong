using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] Collider trigger;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DelayedTrigger", 1f);
    }

    void DelayedTrigger()
    {
        trigger.enabled = true;
    }
}
