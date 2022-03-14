using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float Force = 100f;
    public AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerStay(Collider collider)
    {

        if (collider.GetComponent<Rigidbody>())
        {
            if (!audioSource.isPlaying)
            { audioSource.Play(); }
            collider.GetComponent<Rigidbody>().AddForce(transform.up * Force);
        }
    }
}
