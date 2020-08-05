using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks {
    public string gameVersion = "1";

    public byte maxPlayerPerRoom = 8;

    public byte minPlayersPerRoom = 2;

    public GameObject progressLabel;

    public Button playButton;

    public string initialSceneName;

    public bool isConnecting = false;

    public CanvasGroup connectMenu;

    public CanvasGroup roomListMenu;

    public CanvasGroup gamePrepareMenu;

	#region Unity Callbacks
	private void Awake() {
        // Critical, makes sure we are all on the same scene at all times
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start() {
        HideCanvasGroup(roomListMenu);
        HideCanvasGroup(gamePrepareMenu);
    }

	#endregion

	#region PUN Callbacks
	// This the callback that happens when we successfully connect to the Photon network for the first time
	public override void OnConnectedToMaster() {
        Debug.Log("PUN: Connected to Photon Master server");
        // if (isConnecting) {  PhotonNetwork.JoinLobby(); }
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby() {
        Debug.Log("PUN: Joined Lobby");
        
        ShowCanvasGroup(roomListMenu);

        HideCanvasGroup(connectMenu);
    }


    // Called when we enter the room as a regular client or master client
    public override void OnJoinedRoom() {
        Debug.Log("PUN: OnJoinedRoom() called by PUN. Now this client is connected to a room.");
        isConnecting = false;


        HideCanvasGroup(roomListMenu);
        ShowCanvasGroup(gamePrepareMenu);
    }
    
    // Called when someone else enters the room
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("PUN: A new client joined the room!");
        
        // If we don't have enough players, update the UI to let the player know
        if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersPerRoom) {
            progressLabel.GetComponentInChildren<Text>().text = "Waiting for " + (minPlayersPerRoom - PhotonNetwork.CurrentRoom.PlayerCount) + " players...";
        }
        // Otherwise we are good to go
        else {
            // Only the master client should change scenes when we have enough players
            if (PhotonNetwork.IsMasterClient) {

                /*
                CloseRoom();
                // Tell photon to switch scenes to the Room scene
                PhotonNetwork.LoadLevel(initialSceneName);
                */
            }
        }
    }
    
    // What happens if the connection fails?
    // Let the user know and update isConnecting
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN wiht reason {0}", cause);
        progressLabel.GetComponentInChildren<Text>().text = "Connection failed...";
        isConnecting = false;
    }
	#endregion

	#region Custom Functions

	// Public function for our button to start connecting to the main Photon servers
	// Also handles some UI things
	public void StartConnection() {
        playButton.interactable = false;
        progressLabel.SetActive(true);
        isConnecting = true;
        // Connect();
        EnterGame();
    }

    // The internal function that handles different cases for connecting to the Photon severs
    private void Connect() {
        
        // If we come back from the game and already connected to the Photon servers
        if (PhotonNetwork.IsConnected) {
            // Try to join an existing group of clients in a room to play the game
            PhotonNetwork.JoinRandomRoom();
        }
        // We are starting up the game for the first time
        else {
            // Connect to the Photon servers for the first time
            // Will call OnConnectedToMaster() when successfully connected
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            // Setting our game version
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    private void EnterGame() {
        // Connect to the Photon servers for the first time
        // Will call OnConnectedToMaster() when successfully connected
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        // Setting our game version
        PhotonNetwork.GameVersion = gameVersion;

    }

    private void HideCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void ShowCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseRoom() {
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void CreateNewRoom(string roomName) {

        if (PhotonNetwork.IsConnected) {
            // Try to join an existing group of clients in a room to play the game
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
        }
        // We are starting up the game for the first time
        else {
            // Connect to the Photon servers for the first time
            // Will call OnConnectedToMaster() when successfully connected
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            // Setting our game version
            PhotonNetwork.GameVersion = gameVersion;
        }
        // PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public void StartGame() {
        CloseRoom();
        PhotonNetwork.LoadLevel(initialSceneName);
    }
    #endregion
}
