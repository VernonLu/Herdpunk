using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviour {

	#region Singleton
	private static LocalGameManager instance;
    public static LocalGameManager Instance {
        get { return instance; }
    }
    private void Awake() {
        if (null != instance && instance != this) {
            Destroy(gameObject);
        }
        instance = this;
    }
	#endregion


    void Start() {
        HideResult();
        HideImage(loseImage);
        HideImage(winImage);
        HideImage(drawImage);
    }

    // Update is called once per frame
    void Update() {

    }


    /// <summary>
    /// Player leave room and return to lobby
    /// </summary>
    public void LeaveRoom() {
        StartCoroutine(Leave());
    }

    private IEnumerator Leave() {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom) {
            yield return null;
        }
        SceneManager.LoadScene("Lobby");
    }

    public ScoreArea team1ScoreArea;
    public ScoreArea team2ScoreArea;
    public CanvasGroup resultCanvas;


    public Text resultText;

    public CanvasGroup loseImage;
    public CanvasGroup winImage;
    public CanvasGroup drawImage;

    public void HideResult() {
        resultCanvas.alpha = 0;
        resultCanvas.interactable = false;
        resultCanvas.blocksRaycasts = false;
    }

    public void ShowResult() {
        resultCanvas.alpha = 1;
        resultCanvas.interactable = true;
        resultCanvas.blocksRaycasts = true;
    }

    public void EndGame() {
        ShowResult();
        int team1score = team1ScoreArea.GetScore();
        int team2score = team2ScoreArea.GetScore();
        Debug.Log("Team 1 Score: " + team1score);
        Debug.Log("Team 2 Score: " + team2score);
        if(team1score > team2score) {
            if(PlayerData.team == 1) {
                ShowImage(winImage);
            }
            else {
                ShowImage(loseImage);
            }
        }
        else if(team1score < team2score) {
            if (PlayerData.team == 2) {
                ShowImage(winImage);
            }
            else {
                ShowImage(loseImage);
            }
        }
        else {
            ShowImage(drawImage);
        }
    }

    public void HideImage(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0;
    }

    public void ShowImage(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1;
    }


}
