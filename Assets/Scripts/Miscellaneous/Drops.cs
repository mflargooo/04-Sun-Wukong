using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drops : MonoBehaviour
{
    [SerializeField] private GameObject[] drops;
    [SerializeField] private float[] probabilties;

    [SerializeField] private float launchSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Drop()
    {
        for (int i = 0; i < drops.Length; i++)
        {
            if (Random.Range(0f, 1f) < probabilties[i])
            {
                Rigidbody rb = Instantiate(drops[i], transform.position, drops[i].transform.rotation).GetComponent<Rigidbody>();
                int j = 0;

                Vector3 dir = Vector3.zero;
                while (Vector3.Dot(dir = Random.onUnitSphere, Vector3.up) > 0 && j < 1000)
                {
                    j++;
                }

                rb.velocity = launchSpeed * dir;
            }
        }
    }
}
