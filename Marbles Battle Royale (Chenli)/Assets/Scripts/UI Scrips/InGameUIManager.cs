
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
public class InGameUIManager : MonoBehaviour
{
    [Header("InGameUIList & InGameUICanvas need Manually attach: ")]
    public GameObject[] InGameUIList;

    [SerializeField] private PhotonView photonView;
    private PlayerManager playerManager;
    private MovementController movementController;
    private JumpController jumpController;
    private CinemachineFreeLook virtualCamera;
    private AxisState m_XAxis, m_YAxis;
    private RoomManager roomManager;
    [SerializeField] private GameObject InGameUI, ControlTipsUI, ControlTipsNotice;
    [SerializeField] private Image PlayerUIhealthBarImage;
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] DepthOfField df;

    public Slider focusDistanceSlider, aptureSlider, focalLengthSlider;
    private void Awake()
    {

        //Debug.Log("Paraent " + transform.parent);
        photonView = transform.parent.parent.GetComponent<PhotonView>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        if (photonView.IsMine)
        {
            // Billboard.enabled = false; //this make sure player no need to see their own billboard
            Destroy(this.transform.parent.Find("BillBoard").gameObject);
        }
        else
        {
            Destroy(this.transform.parent.Find("ScoreBoard").gameObject);
            Destroy(this.gameObject);//make sure the UI Canvas panel not messed up when play online
        }
        InGameUI = transform.Find("InGameMenu").gameObject;
        ControlTipsUI = transform.Find("ControlTips").gameObject;
        ControlTipsNotice = transform.Find("ControlTipsNotice").gameObject;
        var player = transform.parent.parent;
        movementController = player.GetComponent<MovementController>();
        jumpController = player.GetComponent<JumpController>();
        virtualCamera = player.Find("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
        m_XAxis = virtualCamera.m_XAxis;
        m_YAxis = virtualCamera.m_YAxis;
        playerManager = player.GetComponent<MovementController>().playerManager;
        ControlTipsNotice.SetActive(false);
        PlayerUIhealthBarImage = this.transform.Find("HealthbarBackground/Healthbar").GetComponent<Image>();

        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out df);

        focusDistanceSlider.value = df.focusDistance.value;
        focusDistanceSlider.interactable = false;
        aptureSlider.value = df.aperture.value;
        focalLengthSlider.value = df.focalLength.value;



    }

    void Start()
    {
        if (!roomManager.isTrainingGround) { ContrilTips(); }//otherwise no showup when game start
        InGameMenu();//set menu off when start
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameMenu();
        }
        if (InGameUI.activeSelf) { movementController.rb.angularVelocity = Vector3.zero; }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ContrilTips();
        }
        SetHealthBar(PlayerUIhealthBarImage, playerManager.billboardvalue);

    }
    public void InGameMenu()
    {
        if (InGameUI.activeSelf)//if menu is active, switch it inactive
        {
            InGameUI.SetActive(false);
            virtualCamera.m_YAxis.m_MaxSpeed = m_YAxis.m_MaxSpeed;
            virtualCamera.m_XAxis.m_MaxSpeed = m_XAxis.m_MaxSpeed;
            // movementController.enabled = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            InGameUI.SetActive(true);
            virtualCamera.m_YAxis.m_MaxSpeed = 0;
            virtualCamera.m_XAxis.m_MaxSpeed = 0;
            // movementController.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        jumpController.isMenuOpen = InGameUI.activeSelf;
    }
    public void ContrilTips()
    {

        if (ControlTipsUI.activeSelf)
        {
            ControlTipsNotice.SetActive(true);
            ControlTipsUI.SetActive(false);
        }
        else
        {
            ControlTipsNotice.SetActive(false);
            ControlTipsUI.SetActive(true);
        }
    }
    public void OpenMenu(string menuName)
    {
        //Debug.Log("Go to " + menuName);
        for (int i = 0; i < InGameUIList.Length; i++)
        {
            if (InGameUIList[i].name == menuName)
            {
                // Debug.Log("set " + menuList[i] + "Active");
                InGameUIList[i].SetActive(true);
            }
            else
            {
                // Debug.Log("set " + menuList[i] + "DeActive");
                InGameUIList[i].SetActive(false);
            }
        }
    }
    public static void SetHealthBar(Image healthBarImage, float billboardvalue)
    {
        Color goodHealth = Color.green;
        Color mediumHealth = Color.yellow;
        Color badHealth = Color.red;
        if (billboardvalue >= 0.6f)
        {
            healthBarImage.color = goodHealth;
        }
        else if (billboardvalue < 0.6f && billboardvalue >= 0.3f)
        {
            healthBarImage.color = mediumHealth;
        }
        else { healthBarImage.color = badHealth; }
        healthBarImage.fillAmount = billboardvalue;
    }
    public void LeaveRoom()
    {
        playerManager.LeaveRoom();
        // Destroy(GameObject.Find("RoomManager").gameObject);
        // //PhotonNetwork.Disconnect();
        // // PhotonNetwork.LeaveRoom();
        // //PhotonNetwork.LoadLevel(0);
        // PhotonNetwork.LeaveRoom();
        // //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
        //  SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        // Debug.Log("Leaved Room");
    }
    public void SetFocusDistance()
    {
        df.focusDistance.value = focusDistanceSlider.value;
    }
    public void SetFocalLength()
    {
        df.focalLength.value = focalLengthSlider.value;
    }
    public void SetApture()
    {
        df.aperture.value = aptureSlider.value;
    }
}
