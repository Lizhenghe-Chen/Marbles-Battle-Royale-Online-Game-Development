using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    

    public RoomInfo info;

    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = roomInfo.Name;
    }

    public void OnClick()
    {
        NetworkManager.Instance.JoinRoom(info);
    }
    
}
