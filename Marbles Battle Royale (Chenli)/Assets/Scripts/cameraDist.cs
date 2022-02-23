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
    [SerializeField] float disdance;
    float maxRadius = 5; //should >0

    float minRadius = -10; //should <0

    public float offset_Value = 0.5f; //offset when Mouse ScrollWheel

    private float total_Offset;//distance between player and camera
    private CinemachineFreeLook virtualCamera;
    void Start()
    {
        virtualCamera = this.GetComponent<CinemachineFreeLook>();
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        playerPosition = transform.parent;
    }

    //================================================================
    Vector3 target;

    //PhotonView photonView;
    Rigidbody rg;

    // Update is called once per frame
    void Update()
    {
        disdance = Vector3.Distance(playerPosition.position, transform.position);
        DepthOfField pr;
        postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out pr);
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && total_Offset <= maxRadius)
        {
            for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
            {
                // Debug
                //     .Log(i +
                //     ". " +
                //     virtualCamera.m_Orbits[i].m_Radius +
                //     "" +
                //     virtualCamera.m_Orbits[i].m_Height);
                virtualCamera.m_Orbits[i].m_Radius += offset_Value;
                total_Offset += offset_Value;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && total_Offset >= minRadius)
        {
            for (int i = 0; i < virtualCamera.m_Orbits.Length; i++)
            {
                virtualCamera.m_Orbits[i].m_Radius -= offset_Value;
                total_Offset -= offset_Value;
            }
        }
        pr.focusDistance.value = disdance;
    }
}
