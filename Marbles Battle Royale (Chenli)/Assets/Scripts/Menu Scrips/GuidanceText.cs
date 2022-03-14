using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GuidanceText : MonoBehaviour
{

    [SerializeField] RoomManager RoomManager;
    public int guidanceStepIndex = 0;
    public TMP_Text GuidanceMessage, ReturnMessage, ContinueMessage;
    [TextArea] public string[] message;
    [SerializeField] float MaxTime, totalTime = 0;
    public int currentStage = 0, Goal = 0;
    [SerializeField] Animator aimator;
    private string Q_Return = "Press 'Q' to Return", E_Continue = "Press 'E' to Continue";
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Guidance(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Guidance(-1);
        }



    }

    void Guidance(int action)
    {
        if (guidanceStepIndex <= 0 && action < 0) { return; }
        if (currentStage <= Goal) { guidanceStepIndex += action; }

        switch (guidanceStepIndex)
        {
            case 0:
                aimator.Play("FadeIn", 0, 0);
                // GuidanceMessage.text = "Hello, Welcome To Marbles Battle Royale! Here will be some guidance to tell you what and how this game is going\n Press 'E' to continue.";
                GuidanceMessage.text = message[0];
                ReturnMessage.text = string.Empty;
                ContinueMessage.text = E_Continue;
               
                break;
            case 1:
                aimator.Play("FadeIn", 0, 0);
                GuidanceMessage.text = message[1];
                ReturnMessage.text = Q_Return;
               
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
