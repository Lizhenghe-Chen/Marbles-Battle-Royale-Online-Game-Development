using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
public class Billboard : MonoBehaviour
{
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    [SerializeField] Camera cam;
    [SerializeField] Transform Player;
    [SerializeField] PhotonView playerPhotonView;
    [SerializeField] TMP_Text playerNickname;
    [SerializeField] GameObject UI;

    [SerializeField] Image healthBarImage, PlayerUIhealthBarImage;
    [SerializeField] CollisionDetect collisionDetect_healthBarImage;
    PlayerManager playerManager;
    // [Header("Below Parameters need Manually attach\n")]
    [Tooltip("This means the value of the billboard above the player")][SerializeField] float aboveOffset;
    void Start()
    {
        Player = transform.parent.parent;
        playerPhotonView = transform.parent.parent.GetComponent<PhotonView>();
        playerNickname = transform.Find("PlayerName").GetComponent<TMP_Text>();
        playerNickname.text = playerPhotonView.Owner.NickName;
        UI = transform.parent.Find("Canvas").gameObject;
        PlayerUIhealthBarImage = UI.transform.Find("HealthbarBackground/Healthbar").GetComponent<Image>();
        healthBarImage = transform.Find("showHealthbarBackground/Healthbar").GetComponent<Image>();
        playerManager = transform.parent.parent.GetComponent<MovementController>().playerManager;
        collisionDetect_healthBarImage = transform.parent.parent.GetComponent<CollisionDetect>();
        if (playerPhotonView.IsMine)
        {
            GetComponent<Canvas>().enabled = false; //this make sure player no need to see their own billboard
        }
        else
        {
            Destroy(UI);//make sure the UI panel not messed up when play online
        }
    }
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); }
        if (cam == null) { return; }
        var billboardvalue = playerManager.currentHealth / playerManager.playerHealth;
        //healthBarImage.fillAmount = collisionDetect_healthBarImage.billboardvalue;
        healthBarImage.fillAmount = billboardvalue;
        healthBarImage.color = JudgeColor(billboardvalue);
        // DisplayHealthBar(healthBarImage, billboardvalue);

        BillBoardFollow();
        transform.LookAt(cam.transform);
        if (playerPhotonView.IsMine)
        {
            PlayerUIhealthBarImage.fillAmount = billboardvalue;
            PlayerUIhealthBarImage.color = JudgeColor(billboardvalue);
            //DisplayHealthBar(PlayerUIhealthBarImage, billboardvalue);
        }
    }
    public void BillBoardFollow()
    {

        //float lerpValue = Mathf.Lerp(transform.position, Player.position, 0.0005f);
        // aboveOffset = Player.GetComponent<SphereCollider>().radius * 5f * Player.transform.localScale.y;
        aboveOffset = 3 * Player.transform.localScale.y;
        Vector3 target = new Vector3(Player.position.x, Player.position.y + aboveOffset, Player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 100);
    }

    public static void DisplayHealthBar(Image healthBarImage, float billboardvalue)
    {
        Color goodHealth = Color.green;
        Color mediumHealth = Color.yellow;
        Color badHealth = Color.red;
        Color currentColor = healthBarImage.color;
        //Color.Lerp(currentColor, Color.blue, Mathf.PingPong(Time.time, 1));

        healthBarImage.fillAmount = billboardvalue;
        if (billboardvalue >= 0.6f)
        {
            healthBarImage.color = goodHealth;
        }
        else if (billboardvalue < 0.6f && billboardvalue >= 0.3f)
        {
            healthBarImage.color = mediumHealth;
        }
        else { healthBarImage.color = badHealth; }
    }
    public static Color JudgeColor(float billboardvalue)
    {
        Color goodHealth = Color.green;
        Color mediumHealth = Color.yellow;
        Color badHealth = Color.red;
        //Color.Lerp(currentColor, Color.blue, Mathf.PingPong(Time.time, 1));
        if (billboardvalue >= 0.6f)
        {
            return goodHealth;
        }
        else if (billboardvalue < 0.6f && billboardvalue >= 0.3f)
        {
            return mediumHealth;
        }
        else { return badHealth; }
    }
}
