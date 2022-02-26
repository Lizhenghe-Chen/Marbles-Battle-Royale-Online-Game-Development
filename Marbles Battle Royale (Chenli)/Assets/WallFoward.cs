using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFoward : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.05f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(moveSpeed, 0, 0);
    }
}
