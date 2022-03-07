using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float Force = 100f;
    void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<Rigidbody>())
        {
            collider.GetComponent<Rigidbody>().AddForce(transform.up * Force);
        }
    }
}
