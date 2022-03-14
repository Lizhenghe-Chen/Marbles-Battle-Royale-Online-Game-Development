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


        healthBarImage = transform.Find("showHealthbarBackground/Healthbar").GetComponent<Image>();
        playerManager = transform.parent.parent.GetComponent<MovementController>().playerManager;
        collisionDetect_healthBarImage = transform.parent.parent.GetComponent<CollisionDetect>();
        if (playerPhotonView.IsMine)
        {
            GetComponent<Canvas>().enabled = false; //this make sure player no need to see their own billboard
        }
    }
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); }
        if (cam == null) { return; }
        //healthBarImage.fillAmount = collisionDetect_healthBarImage.billboardvalue;
        healthBarImage.fillAmount = playerManager.billboardvalue;
        CollisionDetect.DisplayHealthBar(healthBarImage, playerManager.billboardvalue);
        BillBoardFollow();
        transform.LookAt(cam.transform);
    }
    public void BillBoardFollow()
    {

        //float lerpValue = Mathf.Lerp(transform.position, Player.position, 0.0005f);
        // aboveOffset = Player.GetComponent<SphereCollider>().radius * 5f * Player.transform.localScale.y;
        aboveOffset = 3 * Player.transform.localScale.y;
        Vector3 target = new Vector3(Player.position.x, Player.position.y + aboveOffset, Player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 100);
    }
}
