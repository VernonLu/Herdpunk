using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAutoDestroy : MonoBehaviourPunCallbacks {
    public float waitTime = 1f;

    // Start is called before the first frame update
    void Start() {
        Invoke("AutoDestroy", waitTime);
    }

    private void AutoDestroy() {
        if (photonView.IsMine) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}