using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerName;

    public Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        playerName.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy (gameObject);
        }
    }

    // public override void OnLeftRoom()
    // {
    //     Destroy (gameObject);
    // }
}
