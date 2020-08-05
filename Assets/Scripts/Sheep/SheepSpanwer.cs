using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SheepSpanwer : MonoBehaviour {

    private enum GenerationType { Area, Point }

    public GameObject sheepPrefab;

    public int startSpawnNumber = 12;

    public bool generating = true;

    [SerializeField]
    private GenerationType generationType = GenerationType.Area;

    public float spawnInterval = 0.5f;
    public int spawnNumber = 1;

    [Header("Area Spawn")]
    public Transform spawnArea;
    public Vector2 generateAreaSize;

    [Header("Point Spawn")]
    public Transform spawnPointParent;
    public List<Transform> spawnPoints;

    void Start() {

        if (!PhotonNetwork.IsMasterClient) {
            Debug.Log("Spanwer: Destroy");
            Destroy(gameObject);
            return;
        }


        spawnPoints = spawnPointParent.GetComponentsInChildren<Transform>().ToList();
        spawnPoints.Remove(spawnPointParent);

        for (int i = 0; i < startSpawnNumber; ++i) {
            GenerateSheep();
        }


        StartCoroutine(GenerateSheepInGame());
    }


    void Update() {

    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawCube(spawnArea.position, new Vector3(generateAreaSize.x, 0.1f, generateAreaSize.y));
    }


    private void GenerateSheep() {
        // Generate random position

        Vector3 pos = new Vector3(0, 0, 0);

        switch (generationType) {
            case GenerationType.Area:
                float x = Random.Range(-generateAreaSize.x / 2, generateAreaSize.x / 2) + spawnArea.position.x;
                float z = Random.Range(-generateAreaSize.y / 2, generateAreaSize.y / 2) + spawnArea.position.z;
                pos = new Vector3(x, spawnArea.position.y, z);
                break;
            case GenerationType.Point:
                pos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
                break;
        }

        // Generate random rotation
        Quaternion rot = Quaternion.identity;

        // Instantiate sheep
        PhotonNetwork.Instantiate(sheepPrefab.name, pos, rot);
    }


    private IEnumerator GenerateSheepInGame() {
        while (generating) {
            yield return new WaitForSeconds(spawnInterval);

            for(int i = 0; i < spawnNumber; ++i) {
                GenerateSheep();
            }
        }
    }
}
