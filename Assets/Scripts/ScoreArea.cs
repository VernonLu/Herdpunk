using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreArea : MonoBehaviourPun {

    public int team;
    public int score { get; private set; }
    public Text scoreText;

    public AudioSource audioSource;
    public AudioClip allyClip;

    public AudioClip enemyClip;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }


    private void OnTriggerEnter(Collider other) {
        Debug.LogWarning(other.tag);
        if (other.CompareTag("FlockAgent")) {


            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("AddScore", RpcTarget.AllBuffered, team);
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }

    public int GetScore() {
        return score;
    }



    [PunRPC]
    public void AddScore(int team) {
        if (team != this.team) { return; }
        score += 1;
        scoreText.text = score.ToString();
        audioSource.PlayOneShot(team == PlayerData.team ? allyClip : enemyClip);
    }
}
