using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float sensitivity = 0.5f;

    public float Damping = 0.5f;

    public bool isDamping = true;

    private float mouse_X = 0f;

    private float mouse_Y = 0f;

    public float minY = 5;

    public float maxY = 180f;

    public float Distance = 5f; //Distance between camera and Player

    public Transform Player;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        mouse_X = Input.GetAxis("Mouse X") * sensitivity * 0.02f; //获取鼠标X轴移动
        mouse_Y = -Input.GetAxis("Mouse Y") * sensitivity * 0.02f; //获取鼠标Y轴移动
        mouse_Y = ClampAngle(mouse_Y, minY, maxY);

        Quaternion mouse_Rotation = Quaternion.Euler(mouse_Y, mouse_X, 0);
        Vector3 mouse_Position =
            mouse_Rotation * new Vector3(0.0f, 2.0f, -Distance) +
            Player.position;
        if (isDamping)
        {
            transform.rotation =
                Quaternion
                    .Slerp(transform.rotation,
                    mouse_Rotation,
                    Time.deltaTime * Damping);
            transform.position =
                Vector3
                    .Lerp(transform.position,
                    mouse_Position,
                    Time.deltaTime * Damping);
        }
        else
        {
            transform.position = mouse_Position;
            transform.rotation = mouse_Rotation;
        }
    }

    void lateUpdate()
    {
    }

    private float ClampAngle(float angle, float minY, float maxY)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, minY, maxY);
    }
}
