using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomName;
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
