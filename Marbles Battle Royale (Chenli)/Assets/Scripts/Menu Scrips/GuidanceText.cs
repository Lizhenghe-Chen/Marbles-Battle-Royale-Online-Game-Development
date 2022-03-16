using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GuidanceText : MonoBehaviour
{

    [SerializeField] RoomManager RoomManager;
    [SerializeField] KeepSetting keepSetting;
    public GameObject LocalPlayer;
    public Transform startPointTransform;
    public Transform[] guidancePoints;
    [Tooltip("guidanceStepIndex means current step, Goal just like a finnished level")]
    public int guidanceStepIndex = 0, Goal = 0;


    public float moventMission, jumpRushMission;
    public TMP_Text GuidanceMessage, ReturnMessage, ContinueMessage;

    //[SerializeField] float MaxTime, totalTime = 0;

    [SerializeField] Animator aimator;
    [TextArea(5, 7)] public string[] message;
    private string Q_Return = "Press 'Q' to Return", E_Continue = "Press 'E' to Continue";
    void Awake()
    {
        RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        GuidanceMessage = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        //  startPoint = startPointTransform.position;
        if (RoomManager.isTrainingGround && keepSetting.showTutorial) { transform.parent.gameObject.SetActive(true); } else transform.parent.gameObject.SetActive(false);
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

        if (Goal == 2)//until below done the goal will be set to 3
        {
            moventMission += Mathf.Abs(Input.GetAxis("Horizontal"));
            moventMission += Mathf.Abs(Input.GetAxis("Vertical"));
            if (moventMission >= 200f) { Goal = 3; guidanceStepIndex = 3; Guidance(0); }
        }
        if (Goal == 3)
        {
            if (jumpRushMission >= 4) { Goal = 4; guidanceStepIndex = 4; Guidance(0); }
        }


    }

    void Guidance(int action)
    {
        if ((guidanceStepIndex <= 0 && action < 0)) { return; }//no less than 0
                                                               // if (currentStage <= Goal) { guidanceStepIndex += action; }
        if (action > 0 && guidanceStepIndex > Goal)
        {
            guidanceStepIndex = Goal;
            Debug.LogWarning("Reach Max Goal"); return;
        }//player can move back


        // if (guidanceStepIndex != Goal) { guidanceStepIndex = Goal; Debug.LogWarning("Mismatched"); return; }//player can move back
        guidanceStepIndex += action;
        if (action < 0)
        {
            Goal = guidanceStepIndex;//return to previous

        }//player can move back
        switch (guidanceStepIndex)
        {
            case (0):
                ShowText(0, true, false);
                break;
            case (1):
                ShowText(1, false, false);
                break;
            case (2)://Movement Mission Set
                LocalPlayer.transform.position = guidancePoints[0].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                moventMission = 0;
                ShowText(2, false, true);
                break;
            case (3)://Movement Mission Completed and Set Jump Rush Mission

                jumpRushMission = 0;
                if (Goal != 3) { guidanceStepIndex = Goal; break; }
                LocalPlayer.transform.position = guidancePoints[1].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ShowText(3, false, true);
                break;
            case (4):// Jump Rush Mission Completed and introduce jumppad and platform

                if (Goal != 4) { guidanceStepIndex = Goal; break; }
                LocalPlayer.transform.position = guidancePoints[1].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(4, false, false);
                break;
            case (5)://introduce MagicCube
                LocalPlayer.transform.position = guidancePoints[2].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(5, false, false);
                break;
            case (6)://introduce transfer cube
                LocalPlayer.transform.position = guidancePoints[3].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(6, false, false);
                break;
            case (7)://introduce health
                LocalPlayer.transform.position = guidancePoints[4].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(7, false, false);
                break;
            case (8):
                LocalPlayer.transform.position = guidancePoints[5].position;
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(8, false, false);
                break;
            case (9):
                LocalPlayer.GetComponent<MovementController>().playerManager.deathCount = 99;
                LocalPlayer.GetComponent<MovementController>().playerManager.Die();
                LocalPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ShowText(9, false, false);
                ContinueMessage.text = "Exit Guidance Board";
                break;
            case (10):
                ShowText(10, true, true);
                Invoke("HideToturial", 10f);
                break;



                // default:
                //     GuidanceMessage.text = "Hello";
                //     break;
        }

    }
    public void ShowText(int messageIndex, bool hideQ, bool hideE)
    {
        aimator.Play("FadeIn", 0, 0);

        GuidanceMessage.text = message[messageIndex];

        Goal = messageIndex;

        ContinueMessage.text = (hideQ) ? string.Empty : Q_Return;
        ContinueMessage.text = (hideE) ? string.Empty : E_Continue;
    }
    void HideToturial()
    {
        transform.parent.gameObject.SetActive(false);
    }
    // public char[] toCharacters;
    // public string temp;
    // public int index;
    // void TrickOutPut(string message)
    // {
    //     toCharacters = message.ToCharArray();

    //     index = 0;
    //     foreach (char ch in toCharacters)
    //     {

    //         AddString();
    //         index++;

    //     }

    //     temp = string.Empty;


    // }
    // void AddString()
    // {
    //     GuidanceMessage.text += toCharacters[index].ToString();
    // }
}
