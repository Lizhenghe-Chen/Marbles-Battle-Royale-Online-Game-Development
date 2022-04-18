using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Pun;
public class MenuManager : MonoBehaviour
{
    public TMP_Text PlayerSelection;
    [SerializeField] TMP_InputField userNameInput, roomNameInput;

    [SerializeField] Animator aimator;
    public GameObject[] menuList;
    public bool checkInfoOK = false;
    public bool errorColorRed = true;
    public string playerType;

    [SerializeField] GameObject TitledMenu;
    NetworkManager networkManager;

    public AudioSource buttonSound;

    public TMP_Text characterDetails;
    //[TextArea(5, 7)] public string[] message;
    public void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        TitledMenu = networkManager.Start_Debug_Meun;
        userNameInput = networkManager.userNameInput;
        roomNameInput = networkManager.roomNameInput;
        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();
    }
    void Start()
    {
        //GameObject prefab = (GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Marble"), typeof(GameObject));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        SelectPlayer();
    }

    public void OpenMenu(GameObject menuName)
    {
        CheckInfo();
        if (!checkInfoOK && menuName != TitledMenu && menuName.name != "LoadingMenu")
        {
            aimator.Play("Warning", 0, 0);
            return;
            // Debug.LogError("Play");
        }
        //Debug.Log("Go to " + menuName);
        foreach (GameObject menu in menuList)
        {
            if (menu == menuName)
            {
                //    if (menu.name == "TitleMenu") { roomNameInput.text = "TestRoom"; }
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }
    public void OpenConnectionFailedMenu(GameObject menuName)
    {
        foreach (GameObject menu in menuList)
        {
            if (menu == menuName)
            {
                //    if (menu.name == "TitleMenu") { roomNameInput.text = "TestRoom"; }
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }

    public void CheckInfo()
    {

        if (string.IsNullOrEmpty(playerType))
        {
            PlayerSelection.text = "Please select a player first!";
            checkInfoOK = false;
        }
        else if (string.IsNullOrEmpty(userNameInput.text))
        {
            PlayerSelection.text = "Name cannot NULL !";
            checkInfoOK = false;
        }
        else { checkInfoOK = true; }
        if (!checkInfoOK)
        {
            PlayerSelection.color = Color.red;
        }
        else
        {
            networkManager.keepSetting.playerName = userNameInput.text;
            networkManager.keepSetting.playerType = playerType;
            PlayerSelection.color = Color.white;
            PlayerSelection.text = playerType;
            PhotonNetwork.NickName = userNameInput.text;
        }

    }
    void SelectPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //the ray shoot from camera throught the mousePosition
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Select stage
                    if (hit.transform && hit.transform.tag == "Player")
                    {
                        buttonSound.Play();
                        playerType = hit.transform.gameObject.name;
                        characterDetails.text = GenerateIntro(playerType);
                    }
                }
                CheckInfo();
            }
        }
    }
    public string GenerateIntro(string playerType)
    {
        GameObject prefab = (GameObject)Resources.Load(Path.Combine("PhotonPrefabs", playerType), typeof(GameObject));
        string message = prefab.name + "\n";

        message += "Move Speed:\t" + prefab.GetComponent<MovementController>().initial_torque + "\n";
        message += "Rush Force:\t" + prefab.GetComponent<JumpController>().rushForce + "\n";
        message += "Jump Force:\t" + prefab.GetComponent<JumpController>().jumpForce + "\n";
        message += "Mass: \t\t" + prefab.GetComponent<Rigidbody>().mass * 10 + "\n";
        message += "**Mass and Speed will Link to final Damage**";
        return message;
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
