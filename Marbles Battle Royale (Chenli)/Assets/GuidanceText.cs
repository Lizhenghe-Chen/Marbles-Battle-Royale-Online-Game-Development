using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GuidanceText : MonoBehaviour
{

    [SerializeField] RoomManager RoomManager;
    public int guidanceStepIndex = 0;
    TMP_Text GuidanceMessage;
    [SerializeField] float MaxTime, totalTime = 0;
    void Awake()
    {
        RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        GuidanceMessage = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        if (RoomManager.isTrainingGround) { gameObject.SetActive(true); } else gameObject.SetActive(false);
        Guidance(0);//start the guidance
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Guidance(1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Guidance(-1);
        }


     
    }

    void Guidance(int action)
    {
        if (guidanceStepIndex <= 0 && action < 0) { return; }
        guidanceStepIndex += action;
        switch (guidanceStepIndex)
        {
            case 0:
               GuidanceMessage.text = "Hello, Welcome To Marbles Battle Royale! Here will be some guidance to tell you what and how this game is going\n Press 'R' to continue.";
              
                break;
            case 1:
                GuidanceMessage.text = "Just Like the other Battle Royale game, this game is...\n Press 'R' to continue; 'B' back to the previous";
                break;
                // default:
                //     GuidanceMessage.text = "Hello";
                //     break;
        }

    }
    public char[] toCharacters;
    public string temp;
    public int index;
    void TrickOutPut(string message)
    {
        toCharacters = message.ToCharArray();

        index = 0;
        foreach (char ch in toCharacters)
        {

            AddString();
            index++;

        }

        temp = string.Empty;


    }
    void AddString()
    {
        GuidanceMessage.text += toCharacters[index].ToString();
    }
}
