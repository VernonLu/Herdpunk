using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour {

    public RoomInfo roomInfo;
    public Toggle toggle;
    public RoomListMenu roomListMenu;
    public string roomName;
    public Text nameText;

    public void SetRoomInfo(RoomInfo info) {
        roomInfo = info;
        roomName = info.Name;
        nameText.text = info.Name;
        toggle.onValueChanged.AddListener(SelectRoom);
    }

    public void SelectRoom(bool value) {
        // Debug.Log("Room" + value);
        roomListMenu.SelectRoom(value ? roomName : "");
    }

    void Start() {
        toggle.onValueChanged.AddListener(SelectRoom);
    }

    // Update is called once per frame
    void Update() {

    }
}
