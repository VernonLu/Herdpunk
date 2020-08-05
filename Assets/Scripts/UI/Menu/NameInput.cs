using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Reference:
// https://www.youtube.com/watch?v=gxUCMOlISeQ&t=349s

public class NameInput : MonoBehaviour
{
    public InputField inputfield;
    public Button startButton;
    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start()
    {
        startButton.interactable = false;
        // First time launching
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {

        }

        // already has a player name
        else
        {
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            inputfield.text = defaultName;
        }
    }

    public void SetPlayerName()
    {
        string name = inputfield.text;
        //print("Name is " + name);
        startButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        PhotonNetwork.NickName = inputfield.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, inputfield.text);
    }
}
