using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maincamera : MonoBehaviour
{
    public Transform CenObj; //围绕的物体

    private Vector3 Rotion_Transform;

    public float maxDistance = 6f;

    public float sensitivity = 5f;

    public Transform Camera;

    public float m_minimumY = -45f;

    public float m_maximumY = 45f;

    float m_rotationY = 0f;

    float m_rotationX;

    void Start()
    {
    }

    void Update()
    {
        Vector3 V_Camera = Camera.position;

        Vector3 V_Player = CenObj.position;
        Rotion_Transform = CenObj.position;
transform.LookAt (CenObj);
        //  Vector3 targetDir = V_Camera - V_Player;
        // Debug.Log(Vector3.Angle(targetDir, V_Player));
        Ctrl_Cam_Move();
        //Cam_Ctrl_Rotation();
        // TESTCam_Ctrl_Rotation();
        // newRotation();

        //设置即可
    }

    //镜头的远离和接近
    public void Ctrl_Cam_Move()
    {
        float currentDistance =
            Vector3.Distance(Camera.position, CenObj.position);

        Debug
            .Log("Camera:" +
            currentDistance +
            "Follower:" +
            Keep.currentDistance);
        if (currentDistance > maxDistance)
        {
            transform.Translate(Vector3.forward * 0.01234f);
        }
        if (currentDistance > 2 * maxDistance)
        {
            float distance = currentDistance - maxDistance;
            transform.Translate(Vector3.forward * distance * 0.01234f);
        }
        if ((Camera.position.y - CenObj.position.y) < maxDistance * 0.35f)
        {
            transform.Translate(Vector3.up * 0.001234f);
        }
        if ((Camera.position.y - CenObj.position.y) <= 0f)
        {
            transform
                .Translate(Vector3.up *
                (Camera.position.y - CenObj.position.y) *
                0.00123f);
        }
        if ((Camera.position.y - CenObj.position.y) >= maxDistance * 0.75f)
        {
            Invoke("MethodName", 5);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // transform.Translate(Vector3.forward * 0.5f); //速度可调  自行调整
            maxDistance += 0.5f; //速度可调  自行调整w
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0&& maxDistance>5)
        {
            // transform.Translate(Vector3.forward * -0.5f); //速度可调  自行调整
            maxDistance -= 0.5f; //速度可调  自行调整
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "ground")
        {
            //  Debug.Log("Camera touch the ground");
            transform.Translate(Vector3.up * 0.1f);
        }
    }

    //摄像机的旋转
    public void Cam_Ctrl_Rotation()
    {
        var mouse_x = Input.GetAxis("Mouse X"); //获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y"); //获取鼠标Y轴移动

        if (transform.position.y <= 0)
        {
            transform
                .RotateAround(Rotion_Transform,
                transform.right,
                0.1f * sensitivity);
        }
        else
        {
            transform
                .RotateAround(Rotion_Transform,
                transform.right,
                mouse_y * sensitivity);

            transform
                .RotateAround(Rotion_Transform,
                transform.up,
                mouse_x * sensitivity);
            transform.LookAt (CenObj);
        }
    }

    public void TESTCam_Ctrl_Rotation()
    {
          var mouse_x = Input.GetAxis("Mouse X"); //获取鼠标X轴移动
           m_rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);
            transform.localEulerAngles =
            new Vector3(-m_rotationY, transform.localEulerAngles.y, 0);
            transform
                .RotateAround(Rotion_Transform,
                transform.up,
                mouse_x * sensitivity);
            // transform.LookAt (CenObj);
    }

    private void MethodName()
    {
        transform.Translate(-Vector3.up * 0.001f);
    }

    private void newRotation()
    {
        m_rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);
        m_rotationX += Input.GetAxis("Mouse X") * sensitivity;
        transform.localEulerAngles =
            new Vector3(-m_rotationY, transform.localEulerAngles.y, 0);

        //  m_rotationX = Mathf.Clamp(m_rotationX, m_minimumX, m_maximumX);
    }
}
