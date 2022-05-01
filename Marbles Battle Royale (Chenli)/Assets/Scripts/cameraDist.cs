using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
//This File shoulbe attach to Camera
public class cameraDist : MonoBehaviour
{
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] InGameUIManager inGameUIManager;
    [SerializeField] DepthOfField df;
    [SerializeField] Transform player;
    [SerializeField] float disdance; //for postProcessVolume Depth od Field use
    [SerializeField] float playerRadius;
    float maxRadius = 10; //should >0
    float minRadius = -2; //should <0
    int offset_Value = 1; //offset when Mouse ScrollWheel


    private CinemachineFreeLook virtualCamera;
    [SerializeField] float total_Offset, smoothSpeed = 2f;//distance between player and camera
    [SerializeField] float[] currentRadius = { 0, 0, 0 }, currentHeight = { 0, 0, 0 };//the size should same as virtualCamera.m_Orbits.Length


    void Start()
    {
        virtualCamera = this.GetComponent<CinemachineFreeLook>();
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        player = transform.parent;
        inGameUIManager = player.Find("UI/Canvas").GetComponent<InGameUIManager>();
        playerRadius = player.GetComponent<SphereCollider>().radius;
        for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
        {
            currentRadius[i] = virtualCamera.m_Orbits[i].m_Radius;
            currentHeight[i] = virtualCamera.m_Orbits[i].m_Height;
        }

        postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out df);

    }
    void Update()
    {
        ScrollWheeldetect();
        if (virtualCamera.m_Orbits[1].m_Radius != currentRadius[1] && virtualCamera.m_Orbits[1].m_Height != currentHeight[1]) { SoftMoveCamera(); }

        UpdatePS();
    }
    void UpdatePS()//post-processing
    {
        //  disdance = Vector3.Distance(playerPosition.position, transform.position);
        inGameUIManager.focusDistanceSlider.value = df.focusDistance.value = Vector3.Distance(player.position, transform.position);
    }
    void ScrollWheeldetect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && total_Offset < maxRadius)
        {
            //SetDistance(1);
            currentRadius[0] += offset_Value * 0.1f;
            currentRadius[1] += offset_Value;
            currentRadius[2] += offset_Value;
            for (int i = 0; i < currentRadius.Length - 1; i++)
            {
                //currentRadius[i] += offset_Value;
                currentHeight[i] += offset_Value;
            }
            total_Offset += offset_Value;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && total_Offset > minRadius)
        {
            currentRadius[0] -= offset_Value * 0.1f;
            currentRadius[1] -= offset_Value;
            currentRadius[2] -= offset_Value;
            for (int i = 0; i < currentRadius.Length - 1; i++)
            {
                //currentRadius[i] -= offset_Value;
                currentHeight[i] -= offset_Value;
            }
            total_Offset -= offset_Value;
            //SetDistance(-1);
        }

    }
    void SoftMoveCamera()
    {
        for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
        {
            virtualCamera.m_Orbits[i].m_Radius = Mathf.Lerp(virtualCamera.m_Orbits[i].m_Radius, currentRadius[i], Time.deltaTime * smoothSpeed);
            virtualCamera.m_Orbits[i].m_Height = Mathf.Lerp(virtualCamera.m_Orbits[i].m_Height, currentHeight[i], Time.deltaTime * smoothSpeed);
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
