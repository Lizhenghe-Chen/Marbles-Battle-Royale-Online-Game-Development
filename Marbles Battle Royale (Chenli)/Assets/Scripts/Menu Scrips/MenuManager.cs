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
    [SerializeField] TMP_Text ErrorText;
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
        if (SceneManager.GetActiveScene().buildIndex == 0 && TitledMenu.activeSelf) { SelectPlayer(); }
    }

    public void OpenMenu(GameObject menuName)
    {
        CheckInfo();//check if the info (Nickname, character selection) are correct
        if (!checkInfoOK && menuName != TitledMenu && menuName != menuList[0])
        {
            return;
        }
        //each time refresh the whole menu list, close all the menus except the one want to open:
        foreach (GameObject menu in menuList)
        {
            if (menu == menuName) { menu.SetActive(true); }
            else menu.SetActive(false); //close all the menus except the one want to open
        }
    }
    //open error menu
    public void OpenErrorMenu(string errorMessage)
    {
        Debug.Log("OpenErrorMenu");
        ErrorText.text = errorMessage;
        OpenMenu(menuList[4]);
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
            aimator.Play("Warning", 0, 0);
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //the ray shoot from camera throught the mousePosition
            // RaycastHit[] m_Results = new RaycastHit[1];
            // Physics.RaycastNonAlloc(ray, m_Results);

            // if (m_Results[0].collider.gameObject.tag == "Player")
            // {
            //     buttonSound.Play();
            //     playerType = m_Results[0].collider.gameObject.name;
            //     characterDetails.text = GenerateIntro(playerType);
            // }
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform && hit.collider.tag == "Player")//if hit object's collider and is tagged as "Player" 
                {
                    buttonSound.Play();
                    playerType = hit.transform.gameObject.name;
                    characterDetails.text = GenerateIntro(playerType);
                }
            }
            CheckInfo();
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
