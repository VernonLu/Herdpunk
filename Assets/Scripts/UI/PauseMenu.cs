using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    private delegate void Actions();
    private Actions escAction;

    [Header("UI")]
    [Tooltip("pause menu")] public CanvasGroup menu;

    void Start() {
        HideMenu();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) { escAction(); }
    }


    /// <summary>
    /// Show in-game menu
    /// </summary>
    public void ShowMenu() {
        menu.alpha = 1;
        menu.interactable = true;
        menu.blocksRaycasts = true;
        escAction = HideMenu;
    }

    /// <summary>
    /// Hide in-game menu
    /// </summary>
    public void HideMenu() {
        menu.alpha = 0;
        menu.interactable = false;
        menu.blocksRaycasts = false;
        escAction = ShowMenu;
    }

    public void Quit() {
        Application.Quit();
    }
}
