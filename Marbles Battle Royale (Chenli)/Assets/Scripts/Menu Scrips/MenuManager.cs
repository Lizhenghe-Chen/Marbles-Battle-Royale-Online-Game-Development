using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMP_Text PlayerSelection;
    [SerializeField] TMP_InputField userNameInput, roomNameInput;

    [SerializeField] Animator aimator;
    public GameObject[] menuList;
    public bool checkInfoOK = false;

    public string playerType;

    [SerializeField] GameObject TitledMenu;
    NetworkManager networkManager;
    public bool errorColorRed = false;
    public AudioSource buttonSound;
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        TitledMenu = networkManager.Start_Debug_Meun;
        userNameInput = networkManager.userNameInput;
        roomNameInput = networkManager.roomNameInput;
        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

        CheckInfo();
        SelectPlayer();
    }

    public void OpenMenu(GameObject menuName)
    {
        if (!checkInfoOK && menuName != TitledMenu)
        {
          //  Debug.LogError("checkInfoNOTOK");
            aimator.Play("Warning", 0, 0);
        }
        //Debug.Log("Go to " + menuName);
        foreach (GameObject menu in menuList)
        {
            if (menu == menuName)
            {
                if (menu.name == "TitleMenu") { roomNameInput.text = "TestRoom"; }
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
        // for (int i = 0; i < menuList.Length; i++)
        // {
        //     if (menuList[i] == menuName)
        //     {


        //         // Debug.Log("set " + menuList[i] + "Active");
        //         menuList[i].SetActive(true);
        //     }
        //     else
        //     {
        //         // Debug.Log("set " + menuList[i] + "DeActive");
        //         menuList[i].SetActive(false);
        //     }
        // }
    }

    public void CheckInfo()
    {
        if (string.IsNullOrEmpty(playerType))
        {
            PlayerSelection.color = Color.red;
            PlayerSelection.text = "Please select a player first!";
            OpenMenu(TitledMenu);
            checkInfoOK = false;
        }
        else if (string.IsNullOrEmpty(userNameInput.text))
        {
            PlayerSelection.color = Color.red;
            PlayerSelection.text = "Name cannot NULL !";
            OpenMenu(TitledMenu);
            checkInfoOK = false;
        }
        else if (errorColorRed)
        {
            PlayerSelection.color = Color.red;
            checkInfoOK = false;
        }
        else
        {
            networkManager.keepSetting.playerName = userNameInput.text;
            networkManager.keepSetting.playerType = playerType;
            PlayerSelection.color = Color.white;
            PlayerSelection.text = "Selected Player: " + playerType;
            checkInfoOK = true;
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
                        errorColorRed = false;
                        playerType = hit.transform.gameObject.name;
                        //   GameObject.Find("RoomManager").GetComponent<RoomManager>().playerType = playerType;
                        //   Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
        }
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
