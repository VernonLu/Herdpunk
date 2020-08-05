using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviourPunCallbacks {
    public PlayerSpawner playerSpawner;
    public LocalGameManager gameManager;

    private bool start = false;
    private float timerIncrementValue;
    private float startTime;
    
    [SerializeField] private float timer = 120;
    public float countdown;
    ExitGames.Client.Photon.Hashtable CustomeValue;
    
    public Text timerText;
    public Animator animator;

    private void Awake() {
        start = false;
    }
    void Start() {
        if (PhotonNetwork.IsMasterClient) {
            StartTimer();
        }
        ShowTime(timer);
    }

    void Update() {

        if (!start) return;

        timerIncrementValue = (float)PhotonNetwork.Time - startTime;

        ShowTime(timer - timerIncrementValue);

        if(timerIncrementValue >= timer - countdown)
        {
            print("Countdown");
            animator.SetBool("countdown", true);
        }

        if (timerIncrementValue >= timer) {
            //Timer Completed
            StopTimer();
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
        if (propertiesThatChanged.ContainsKey("start")) {

            start = bool.Parse(propertiesThatChanged["start"].ToString());

            if (start) {
                startTime = float.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                playerSpawner.localPlayer.GrantControl();

            }
            else {
                playerSpawner.localPlayer.RetrieveControl();
                gameManager.EndGame();
            }
        }
    }

    public void StartTimer() {
        if (!PhotonNetwork.IsMasterClient) { return; }

        CustomeValue = new ExitGames.Client.Photon.Hashtable {
            { "start", true },
            { "StartTime", PhotonNetwork.Time }
        };
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
    }

    public void StopTimer() {

        if (!PhotonNetwork.IsMasterClient) { return; }

        CustomeValue = new ExitGames.Client.Photon.Hashtable {
            { "start", false },
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
    }

    public void ShowTime(float time) {
        int timeInt = Mathf.CeilToInt(time);

        int min = Mathf.FloorToInt(timeInt / 60);
        string minText = min < 10 ? "0" + min.ToString() : min.ToString();

        int sec = Mathf.FloorToInt(timeInt % 60);
        string secText = sec < 10 ? "0" + sec.ToString() : sec.ToString();

        timerText.text = minText + ":" + secText;
    }
}
