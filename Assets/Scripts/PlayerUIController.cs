using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUIController : MonoBehaviourPun
{
    public Image dashCD_img;
    public Image knockout;

    public GameObject stunEffect;

    public Text nametag;

    public PlayerController player;

    private Color knockoutOriginalColor;


    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        knockoutOriginalColor = knockout.color;

        DisableEffect(stunEffect);
    }

    private void Update()
    {
        transform.position = player.transform.position;
    }


    [PunRPC]
    public void SetPlayerRef(int playerViewID)
    {
        GameObject[]players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject i in players)
        {
            PlayerController playerctrl = i.GetComponent<PlayerController>();
            if (i.GetComponent<PhotonView>().ViewID == playerViewID)
            {
                player = playerctrl;
                nametag.text = i.GetComponent<PhotonView>().Owner.NickName;
            }
        }
    }

    [PunRPC]
    public void ToggleKnockoutVisual(bool activate)
    {
        // Toggle on
        if (activate) {
            EnableEffect(stunEffect);
        }
        // Toggle off
        else {
            DisableEffect(stunEffect);
        }
    }


    private void EnableEffect(GameObject effect) {
        effect.SetActive(true);
    }

    private void DisableEffect(GameObject effect) {
        effect.SetActive(false);

    }
}
