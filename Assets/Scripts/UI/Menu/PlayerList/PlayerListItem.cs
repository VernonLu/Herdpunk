using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour {

    public Text playerText;

    public Player player;

    private bool isReady = false;

    public Toggle isReadyToggle;

    public Toggle isMineToggle;

    public void SetPlayerInfo(Player playerInfo) {
        player = playerInfo;
        playerText.text = player.NickName;
    }

    public void ToggleReady(bool value) {
        isReady = value;
        isReadyToggle.isOn = value;
    }

    /// <summary>
    /// Get status of current player (ready or not)
    /// </summary>
    /// <returns></returns>
    public bool GetIsReady() {
        return isReady;
    }


    /// <summary>
    /// Set this item belongs to local player
    /// </summary>
    public void IsMine() {
        isMineToggle.isOn = true;
    }
}
