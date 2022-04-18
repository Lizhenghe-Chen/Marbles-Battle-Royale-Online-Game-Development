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

    [SerializeField] Image healthBarImage;
    // [SerializeField] CollisionDetect collisionDetect_healthBarImage;
    [SerializeField] PlayerManager playerManager;
    // [Header("Below Parameters need Manually attach\n")]
    // [Tooltip("This means the value of the billboard above the player")]
    // [SerializeField] private float aboveOffset;
    void Start()
    {
        Player = transform.parent.parent;
        playerPhotonView = transform.parent.parent.GetComponent<PhotonView>();
        playerNickname = transform.Find("PlayerName").GetComponent<TMP_Text>();
        playerNickname.text = playerPhotonView.Owner.NickName;

        healthBarImage = transform.Find("showHealthbarBackground/Healthbar").GetComponent<Image>();
        playerManager = transform.parent.parent.GetComponent<MovementController>().playerManager;
        //  collisionDetect_healthBarImage = transform.parent.parent.GetComponent<CollisionDetect>();

    }
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); }
        if (cam == null) { return; }
        //healthBarImage.fillAmount = collisionDetect_healthBarImage.billboardvalue;

        BillBoardFollow(Player, this.transform, 3);
        transform.LookAt(cam.transform);
        InGameUIManager.SetHealthBar(healthBarImage, playerManager.billboardvalue);
    }
    public static void BillBoardFollow(Transform Player, Transform _this, int offsetMultiplier)
    {
        _this.position = Vector3.MoveTowards(_this.position,
        new Vector3(Player.position.x,
        Player.position.y + offsetMultiplier * Player.transform.localScale.y,
        Player.position.z), 100);
    }

    // public static void DisplayHealthBar(Image healthBarImage, float billboardvalue)
    // {
    //     Color goodHealth = Color.green;
    //     Color mediumHealth = Color.yellow;
    //     Color badHealth = Color.red;
    //     Color currentColor = healthBarImage.color;
    //     //Color.Lerp(currentColor, Color.blue, Mathf.PingPong(Time.time, 1));

    //     healthBarImage.fillAmount = billboardvalue;
    //     if (billboardvalue >= 0.6f)
    //     {
    //         healthBarImage.color = goodHealth;
    //     }
    //     else if (billboardvalue < 0.6f && billboardvalue >= 0.3f)
    //     {
    //         healthBarImage.color = mediumHealth;
    //     }
    //     else { healthBarImage.color = badHealth; }
    // }

}
