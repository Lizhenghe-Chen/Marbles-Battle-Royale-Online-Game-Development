using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Camera cam;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); return; }
        // if (cam == null) { return; }
        transform.LookAt(cam.transform);
    }
}
