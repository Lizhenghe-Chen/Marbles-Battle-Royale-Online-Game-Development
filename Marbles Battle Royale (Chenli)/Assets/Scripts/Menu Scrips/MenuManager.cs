using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMP_Text PlayerSelection;
    [SerializeField] TMP_InputField userNameInput;

    [SerializeField]
    public GameObject[] menuList;

    private string playerType;

    GameObject TitledMenu;
    public bool isNameDublicated = false;
    void Start()
    {
        TitledMenu = GetComponent<NetworkManager>().TitleMenu;
        userNameInput = GetComponent<NetworkManager>().userNameInput;
        Cursor.visible = true;
    }

    void Update()
    {

        CheckInfo();
        SelectPlayer();
    }

    public void OpenMenu(GameObject menuName)
    {
        //Debug.Log("Go to " + menuName);
        foreach (GameObject menu in menuList)
        {
            if (menu == menuName)
            {
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

    void CheckInfo()
    {
        if (playerType == null)
        {
            PlayerSelection.color = Color.red;
            PlayerSelection.text = "Please select a player first!";
            OpenMenu(TitledMenu);
        }
        else if (string.IsNullOrEmpty(userNameInput.text))
        {
            PlayerSelection.color = Color.red;
            PlayerSelection.text = "Name cannot NULL !";
            OpenMenu(TitledMenu);
        }
        else if (isNameDublicated) { PlayerSelection.color = Color.red; }
        else
        {
            PlayerSelection.color = Color.white;
            PlayerSelection.text = "Selected Player: " + playerType;
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
                        isNameDublicated = false;
                        playerType = hit.transform.gameObject.name;
                        GameObject.Find("RoomManager").GetComponent<RoomManager>().playerType = playerType;
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