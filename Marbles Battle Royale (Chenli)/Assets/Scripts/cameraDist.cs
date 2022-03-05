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
    float maxRadius = 5; //should >0
    float minRadius = -10; //should <0
    float offset_Value = 0.2f; //offset when Mouse ScrollWheel

    private float total_Offset;//distance between player and camera
    private CinemachineFreeLook virtualCamera;

    void Start()
    {
        virtualCamera = this.GetComponent<CinemachineFreeLook>();
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        playerPosition = transform.parent;
        playerRadius = playerPosition.GetComponent<SphereCollider>().radius;

    }
    void Update()
    {
        ScrollWheeldetect();
        UpdatePS();
    }
    void UpdatePS()
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
            SetDistance(1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && total_Offset > minRadius)
        {
            SetDistance(-1);
        }
    }
    void SetDistance(int direction)
    {
        for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
        {
            virtualCamera.m_Orbits[i].m_Radius += direction * offset_Value;
            total_Offset += direction * offset_Value;
        }
    }
}
