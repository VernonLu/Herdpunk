using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GamePrepareMenu : MonoBehaviourPunCallbacks {
	#region TeamData Class
	[Serializable]
    public class TeamData {
        public RectTransform content;
        public List<PlayerListItem> teammates = new List<PlayerListItem>();
        public int TeammateCount => teammates.Count;
    }
    #endregion

    [Space]
    public bool checkPlayerStatusBeforeStart;

    [Space]
    public LobbyManager lobbyManager;

    public GameObject playerListItemPrefab;
    public Vector2 playerListItemSize = new Vector2(200, 30);


    public Button changeTeamBtn;
    public Button startReadyBtn;
    public Text startReadyBtnText;
    

    [SerializeField]
    private List<TeamData> teams = new List<TeamData>();

    private bool isReady = false;

    private ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();

	#region Custom functions
	public void ChangeTeam() {
        int teamIndex = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString());
        
        if (teams[1 - teamIndex].TeammateCount + 1 > teams[teamIndex].TeammateCount) {
            Debug.Log("Cannot switch the other team (has more player)");
            return;
        }
        

        playerProps["Team"] = 1 - teamIndex;

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void CreatePlayerUI(Player player, int teamIndex = -1) {

        // Find the team with least number of players
        int index = 0;
        if(teamIndex == -1) {
            for (int i = 1; i < teams.Count; ++i) {
                index = teams[i].TeammateCount < teams[index].TeammateCount ? i : index;
            }
        }
        else {
            index = teamIndex;
        }

        // Add player to the team
        PlayerListItem newPlayerItem = Instantiate(playerListItemPrefab, teams[index].content).GetComponent<PlayerListItem>();
        newPlayerItem.SetPlayerInfo(player);
        teams[index].teammates.Add(newPlayerItem);

        if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) {
            // Debug.Log("Local player");
            newPlayerItem.IsMine();

            CharacterSelector.Instance.SetCharacterColor(teamIndex);

            SaveTeam();
        }

        UpdateContentSize(index);
    }

    public void GetPlayerIndexAndTeamIndex(Player otherPlayer, out int playerIndex, out int teamIndex) {
        playerIndex = -1;

        teamIndex = -1;

        // Get index of player and index of it's team
        foreach (var team in teams) {
            // Find player index using ActorNumber
            int index = team.teammates.FindIndex(player => player.player.ActorNumber == otherPlayer.ActorNumber);
            if (index != -1) {
                playerIndex = index;
                teamIndex = teams.IndexOf(team);
                break;
            }
        }
    }

    public void RemovePlayer(int playerIndex, int teamIndex) {

        if (playerIndex == -1 || teamIndex == -1) { return; }

        // Debug.Log("TeamIndex: " + teamIndex + " PlayerIndex:" + playerIndex);

        // Remove the player from the list and destroy it's UI
        // Destroy UI
        Destroy(teams[teamIndex].teammates[playerIndex].gameObject);

        // Remove from the list
        teams[teamIndex].teammates.RemoveAt(playerIndex);

        UpdateContentSize(teamIndex);
    }

    public void UpdateContentSize(int index) {
        teams[index].content.sizeDelta = new Vector2(playerListItemSize.x, playerListItemSize.y * teams[index].TeammateCount);
    }

    public void UpdateButton() {
        startReadyBtnText.text = PhotonNetwork.IsMasterClient ? "Start" : "Ready";
        startReadyBtn.onClick.RemoveAllListeners();
        if (PhotonNetwork.IsMasterClient) {
            startReadyBtn.onClick.AddListener(StartGame);
        }
        else {
            startReadyBtn.onClick.AddListener(GetReady);
        }
    }

    /// <summary>
    /// Save local player info (team and number)
    /// </summary>
    public void SaveTeam() {
        GetPlayerIndexAndTeamIndex(PhotonNetwork.LocalPlayer, out int playerIndex, out int teamIndex);

        PlayerData.number = playerIndex;

        PlayerData.team = teamIndex + 1;
    }

    public bool CheckPlayerStatus() {
        int numberCount = teams[0].TeammateCount;

        for (int i = 0; i < teams.Count; ++i) {
            // Check number of each team
            if (teams[i].TeammateCount != numberCount) {
                Debug.Log("Teammate number not match");
                return false;
            }
            for (int j = 0; j < numberCount; ++j) {

                // Skip master client
                if (teams[i].teammates[j].player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) { continue; }

                // Check player status 
                if (!teams[i].teammates[j].GetIsReady()) {
                    Debug.Log("Players not ready");
                    return false;
                }
            }
        }

        return true;
    }

    public void StartGame() {
        if (!checkPlayerStatusBeforeStart || CheckPlayerStatus()) {
            SaveTeam();

            // Start the game
            Debug.Log("Game Started");

            lobbyManager.StartGame();
        }
    }


    public void GetReady() {
        // Update player status
        isReady = !isReady;

        changeTeamBtn.interactable = !isReady;

        // Save playerIndex and teamIndex if player is ready
        if (isReady) { SaveTeam(); }

        GetPlayerIndexAndTeamIndex(PhotonNetwork.LocalPlayer, out int playerIndex, out int teamIndex);

        teams[teamIndex].teammates[playerIndex].ToggleReady(isReady);

        UpdatePlayerProperties("Ready", isReady);
    }

    private void UpdatePlayerProperties(object key, object value) {
        playerProps = new ExitGames.Client.Photon.Hashtable {
            { key, value }
        };

        // OnPlayerPropertiesUpdate will be called after properties are updated
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }
	#endregion

	#region PUN Callbacks

	/// <summary>
	/// Set player team and Create local player UI
	/// </summary>
	public override void OnJoinedRoom() {

        CharacterSelector.Instance.ResetCharacterPos();

        UpdateButton();

        // Create UI for all players in the room && add them to team list.
        List<Player> otherPlayers = PhotonNetwork.PlayerListOthers.ToList();
        Debug.Log(otherPlayers.Count);
        foreach (Player player in otherPlayers) {
            CreatePlayerUI(player, int.Parse(player.CustomProperties["Team"].ToString()));
        }

        // Get index of the team with least member
        int teamIndex = 0;
        for (int i = 1; i < teams.Count; ++i) {
            teamIndex = teams[i].TeammateCount < teams[teamIndex].TeammateCount ? i : teamIndex;
        }

        // Update player team
        UpdatePlayerProperties("Team", teamIndex);

        // Create PlayerUI
        CreatePlayerUI(PhotonNetwork.LocalPlayer, teamIndex);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("Player: " + otherPlayer.NickName + "Left the room.");
        
        GetPlayerIndexAndTeamIndex(otherPlayer, out int playerIndex, out int teamIndex);

        RemovePlayer(playerIndex, teamIndex);

        UpdateButton();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {

        GetPlayerIndexAndTeamIndex(targetPlayer, out int playerIndex, out int teamIndex);

        if (changedProps.ContainsKey("Team")) {
            if(playerIndex != -1 && teamIndex != -1) { RemovePlayer(playerIndex, teamIndex); }

            // Get new teamIndex from changedProps
            teamIndex = int.Parse(changedProps["Team"].ToString());

            Debug.Log("Assign Team: Player: " + targetPlayer.NickName + " Team: " + teamIndex.ToString());

            CreatePlayerUI(targetPlayer, teamIndex);
        }
        else if(changedProps.ContainsKey("Ready")) {
            bool isReady = bool.Parse(changedProps["Ready"].ToString());
            if( teamIndex != -1 && playerIndex != -1) {
                teams[teamIndex].teammates[playerIndex].ToggleReady(isReady);
            }
        }
    }
	#endregion
}
