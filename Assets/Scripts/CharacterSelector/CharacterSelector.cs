using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour {
    public static CharacterSelector Instance { get; private set; }
    private void Awake() {
        if(null != Instance && this != Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Transform previewCamera;
    private Vector3 cameraOffsetPos;
    private Vector3 cameraTargetPos;
    public float smoothTime = 0.1f;

    public string path;
    public List<GameObject> playerModels;
    public int currentIndex = 0;
    [Tooltip("Space between two character models")]
    public Vector3 interval = new Vector3(5f, 0, 0);
    public List<Color> teamColors;

    private List<ShowcaseItem> characterList = new List<ShowcaseItem>();

    void Start() {

        cameraOffsetPos = previewCamera.position;
        cameraTargetPos = cameraOffsetPos;
        characterList = GetComponentsInChildren<ShowcaseItem>().ToList();
        SetCharacterColor(0);
    }

    // Update is called once per frame
    void Update() {
        UpdateCameraPosition();
    }

    /// <summary>
    /// Load all character models
    /// </summary>
    private void LoadModels() {

    }

    /// <summary>
    /// Instantiate all models in the playerModels list
    /// </summary>
    private void SetupModels() {
        for(int i = 0; i < playerModels.Count; ++i) {
            Instantiate(playerModels[i], interval * i, Quaternion.Euler(0, 180f, 0), transform);
        }
    }

    /// <summary>
    /// Select next character
    /// </summary>
    public void SelectNext() {
        currentIndex = (currentIndex + 1) % playerModels.Count;
        UpdateCharacterData();
        UpdateCameraTargetPos();
    }

    /// <summary>
    /// Select previous character
    /// </summary>
    public void SelectPrev() {
        currentIndex = (currentIndex + playerModels.Count - 1) % playerModels.Count;
        UpdateCharacterData();
        UpdateCameraTargetPos();
    }

    public void UpdateCharacterData() {
        PlayerData.characterID = currentIndex;
    }

    /// <summary>
    /// Calculate camera target position
    /// </summary>
    private void UpdateCameraTargetPos() {
        cameraTargetPos = cameraOffsetPos + interval * currentIndex;
        ResetCharacterPos();
    }

    /// <summary>
    /// Move camera to focus on selected character
    /// </summary>
    private void UpdateCameraPosition() {
        previewCamera.position = Vector3.Lerp(previewCamera.position, cameraTargetPos, smoothTime);
    }

    public void SetCharacterColor(int teamIndex) {
        if (teamIndex >= teamColors.Count) {
            return;
        }
        foreach (var character in characterList) {
            character.ChangeColor(teamColors[teamIndex]);
        }
    }

    public void ResetCharacterPos() {
        foreach (var character in characterList) { character.ResetRotation(); }
    }
}
