using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerSpawner : MonoBehaviourPun {

	#region Singleton
	private static PlayerSpawner instance;
    public static PlayerSpawner Instance {
        get { return instance; }
    }
    private void Awake() {
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogWarning("Master");
        }

        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        Debug.LogWarning("Awake");
    }
    #endregion

    public string prefabPath = "Characters/";
	public PlayerController playerPrefab;
    public PlayerUIController playerUIPrefab;
    public DiscoBallController discoBallPrefab;

    public List<Transform> spawnPointsTeam1;
    public List<Transform> spawnPointsTeam2;

    public Color team1Color;
    public Color team2Color;

    public PlayerController localPlayer;

    void Start() {
        Debug.LogWarning("Start Spawn");
        SpawnPlayer();
    }

    private void SpawnPlayer() {
        Debug.Log("Team:" + PlayerData.team);
        Debug.Log("Number:" + PlayerData.number);

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        switch (PlayerData.team) {
            case 1:
                spawnPos = spawnPointsTeam1[PlayerData.number].position;
                spawnRot = spawnPointsTeam1[PlayerData.number].rotation;
                break;
            case 2:
                spawnPos = spawnPointsTeam2[PlayerData.number].position;
                spawnRot = spawnPointsTeam2[PlayerData.number].rotation;
                break;
        }
        // Debug.LogWarning("SpawnPlayer");
        PlayerController playerRef = PhotonNetwork.Instantiate(prefabPath + "Player_" + PlayerData.characterID, spawnPos, spawnRot).GetComponent<PlayerController>();
        PlayerUIController playerUIref = PhotonNetwork.Instantiate(playerUIPrefab.name, spawnPos, spawnRot).GetComponent<PlayerUIController>();

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Spawn 1 Ball");
            DiscoBallController discoBallRef = PhotonNetwork.Instantiate(discoBallPrefab.name, new Vector3(0, 3, 0), spawnRot).GetComponent<DiscoBallController>();
        }

        print(playerRef.photonView.ViewID);

        playerUIref.photonView.RPC("SetPlayerRef", RpcTarget.AllBuffered, playerRef.photonView.ViewID);
        playerRef.photonView.RPC("SetPlayerUIRef", RpcTarget.AllBuffered, playerUIref.photonView.ViewID);
        playerRef.photonView.RPC("SetPlayerColor", RpcTarget.AllBuffered, playerRef.photonView.ViewID, PlayerData.team);


        localPlayer = playerRef;
    }

    public Color GetColor(int team) {
        switch (team) {
            case 1:
                return team1Color;
            case 2:
                return team2Color;

            default: return team1Color;
        }
    }
}
