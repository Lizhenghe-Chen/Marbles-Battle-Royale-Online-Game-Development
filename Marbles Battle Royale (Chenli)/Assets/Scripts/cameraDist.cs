using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
//This File shoulbe attach to Camera
public class cameraDist : MonoBehaviour
{
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] Transform playerPosition;
    [SerializeField] float disdance; //for postProcessVolume Depth od Field use
    [SerializeField] float playerRadius;
    float maxRadius = 10; //should >0
    float minRadius = -10; //should <0
    int offset_Value = 1; //offset when Mouse ScrollWheel


    private CinemachineFreeLook virtualCamera;
    [SerializeField] float total_Offset, smoothSpeed = 2f;//distance between player and camera
    [SerializeField] float[] currentDistance = { 0, 0, 0 };//the size should same as virtualCamera.m_Orbits.Length
    void Start()
    {
        virtualCamera = this.GetComponent<CinemachineFreeLook>();
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        playerPosition = transform.parent;
        playerRadius = playerPosition.GetComponent<SphereCollider>().radius;
        for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
        {
            currentDistance[i] = virtualCamera.m_Orbits[i].m_Radius;

        }
    }
    void Update()
    {
        ScrollWheeldetect();
        if (virtualCamera.m_Orbits[0].m_Radius != currentDistance[0]) { SoftMoveCamera(); }

        UpdatePS();
    }
    void UpdatePS()//post-processing
    {
        disdance = Vector3.Distance(playerPosition.position, transform.position);
        DepthOfField pr;
        postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out pr);
        pr.focusDistance.value = disdance - playerRadius;
        // pr.focusDistance.value = disdance;
    }
    void ScrollWheeldetect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && total_Offset < maxRadius)
        {
            //SetDistance(1);
            for (int i = 0; i < currentDistance.Length; i++)
            {
                currentDistance[i] = virtualCamera.m_Orbits[i].m_Radius + offset_Value;
            }
            total_Offset += offset_Value;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && total_Offset > minRadius)
        {
            for (int i = 0; i < currentDistance.Length; i++)
            {
                currentDistance[i] = virtualCamera.m_Orbits[i].m_Radius - offset_Value;
            }
            total_Offset -= offset_Value;
            //SetDistance(-1);
        }

    }
    void SoftMoveCamera()
    {
        for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
        {
            virtualCamera.m_Orbits[i].m_Radius = Mathf.Lerp(virtualCamera.m_Orbits[i].m_Radius, currentDistance[i], Time.deltaTime * smoothSpeed);

        }
    }
    // void SetDistance(int direction)
    // {
    //     for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
    //     {
    //         virtualCamera.m_Orbits[i].m_Radius += direction * offset_Value;
    //         total_Offset += direction * offset_Value;
    //     }
    // }

}
