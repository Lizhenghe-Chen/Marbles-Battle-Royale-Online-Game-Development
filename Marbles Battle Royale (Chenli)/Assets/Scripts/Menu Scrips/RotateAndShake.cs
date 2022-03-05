using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndShake : MonoBehaviour
{
    // Start is called before the first frame update
    float radian; // radian
    [Header("Control object rotation:\n")]
    [Tooltip("Set this null if you want to self rotate")][SerializeField] Transform rotateCenter;
    [SerializeField] bool enableRotate = true; // enable
    [SerializeField] bool InvertrotationDirection = false;
    [SerializeField] float RotationSpeed = 10f;

    //================================================================
    [Header("Control object UpDown & LeftRight:\n")]
    [SerializeField] bool enableUpDown = false; // enable
    [SerializeField] bool enableLeftRight = false;
    [SerializeField] bool invertLeftRight = false;
    [SerializeField] bool UpRight = false;
    [SerializeField] bool UpLeft = false;
    [SerializeField] float shakeSpeed = 1f;
    // [SerializeField] float perRadian = 0.03f;
    [SerializeField] float shakeRange = 0.8f;
    Vector3 oldPos;

    void Start()
    {
        oldPos = transform.position;
    }

    // Update is called once per frame


    void Update()
    {
        if (enableRotate) Rotate();

        UPDownLeftRight();

    }

    void UPDownLeftRight()
    {
        if (!enableUpDown && !enableLeftRight && !UpRight && !UpLeft) { return; }

        if (radian > 10f) radian += Time.deltaTime * shakeSpeed; else radian -= Time.deltaTime * shakeSpeed;

        float d = Mathf.Cos(radian) * shakeRange;

        if (enableUpDown) { transform.position = oldPos + new Vector3(0, d, 0); return; }
        if (enableLeftRight)
        {
            if (invertLeftRight) { transform.position = oldPos + new Vector3(0, 0, d); return; }
            else
                transform.position = oldPos + new Vector3(d, 0, 0); return;
        }

        if (UpRight) { transform.position = oldPos + new Vector3(d, -d, 0); return; }
        if (UpLeft) transform.position = oldPos + new Vector3(d, d, 0);

    }

    void Rotate()
    {
        Vector3 center;
        if (!rotateCenter) { center = transform.position; } else { center = rotateCenter.position; }
        if (InvertrotationDirection)
        {
            this
                .transform
                .RotateAround(center, //rotate center
                Vector3.up, //rotate direction
                RotationSpeed * Time.deltaTime); //rotate speed degrees/sec
        }
        else
        {
            this
                .transform
                .RotateAround(center, //rotate center
                Vector3.right, //rotate direction
                RotationSpeed * Time.deltaTime); //rotate speed degrees/sec
        }
    }
}
