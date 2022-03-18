using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
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

    public TMP_Text characterDetails;
    //[TextArea(5, 7)] public string[] message;
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        TitledMenu = networkManager.Start_Debug_Meun;
        userNameInput = networkManager.userNameInput;
        roomNameInput = networkManager.roomNameInput;
        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();

    }
    void Start()
    {
        GameObject prefab = (GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Marble"), typeof(GameObject));
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
            // OpenMenu(TitledMenu);
            checkInfoOK = false;
        }
        else if (string.IsNullOrEmpty(userNameInput.text))
        {
            PlayerSelection.color = Color.red;
            PlayerSelection.text = "Name cannot NULL !";
            // OpenMenu(TitledMenu);
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
                        // var prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/PhotonPrefabs/" + playerType + ".prefab", typeof(GameObject));

                        // GameObject loadedObject = (GameObject)Resources.Load("Assets/Resources/PhotonPrefabs/" + playerType + ".prefab");
                        //Debug.Log(prefab);
                        characterDetails.text = GenerateIntro(playerType);

                        //GameObject newItemInList = Instantiate(Path.Combine("PhotonPrefabs", playerType)) as GameObject;
                        //   GameObject.Find("RoomManager").GetComponent<RoomManager>().playerType = playerType;
                        //   Debug.Log(hit.transform.gameObject.name);
                    }
                }
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
