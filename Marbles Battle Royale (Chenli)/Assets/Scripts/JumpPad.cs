using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float Force = 1000f;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Rigidbody>())
        {
            collider.GetComponent<Rigidbody>().AddForce(Vector3.up * Force);
        }
    }
}