using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListMenu : MonoBehaviourPunCallbacks {
    public LobbyManager lobbyManager;

    public Transform content;
    public GameObject roomListItemPrefab;
    public Vector2 roomListItemSize = new Vector2(200, 30);

    public ToggleGroup toggleGroup;

    public Button joinBtn;
    public Button createBtn;

    private string newRoomName;
    private string selectedRoomName;

    public List<RoomListItem> roomLists = new List<RoomListItem>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        // Update existing rooms

        // Instantiate new room items
        foreach(var roomInfo in roomList) {
            if (roomInfo.RemovedFromList || !roomInfo.IsOpen) {
                int index = roomLists.FindIndex(x => x.roomName == roomInfo.Name);
                if(index != -1) { 
                    Destroy(roomLists[index].gameObject);
                    roomList.RemoveAt(index);
                }
            }
            else {
                // Skip existing room
                int index = roomLists.FindIndex(x => x.roomInfo.Name == roomInfo.Name);
                if(index != -1) { return; }

                RoomListItem newItem = Instantiate(roomListItemPrefab, content).GetComponent<RoomListItem>();
                newItem.SetRoomInfo(roomInfo);
                newItem.toggle.group = toggleGroup;
                newItem.roomListMenu = this;
                roomLists.Add(newItem);
            }
        }

        // Update content size
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(roomListItemSize.x, roomListItemSize.y * roomList.Count);
    }

    public override void OnCreatedRoom() {
        Debug.Log("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Room create Failed: " + message);
    }


    public void SelectRoom(string roomName) {
        selectedRoomName = roomName;
        joinBtn.interactable = (roomName == "") ? false : true;
    }

    public void JoinRoom() {
        PhotonNetwork.JoinRoom(selectedRoomName);
    }

    public void GetNewRoomName(string value) {
        createBtn.interactable = value == "" ? false : true;
        newRoomName = value;
    }

    public void CreateRoom() {
        lobbyManager.CreateNewRoom(newRoomName);
    }
}
