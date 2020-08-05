using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DiscoBallController : MonoBehaviourPun
{
    [Header("Trigger Sphere")]
    public float triggerRadius;

    public Vector3 snapOffset;
    private bool taken = false;
    private SphereCollider attractionTrigger;
    private BoxCollider boxCollider;
    private PlayerController playerRef;
    

    void Start()
    {
        attractionTrigger = GetComponent<SphereCollider>();
        boxCollider = GetComponent<BoxCollider>();
        attractionTrigger.radius = triggerRadius;
    }

    private void Update()
    {
        if (playerRef != null)
        {
            transform.position = playerRef.transform.position + playerRef.transform.forward + new Vector3(0, 3, 0);
        }
    }


    public void DropBallfunc()
    {
        // HOTFIX, check if player ref is null
        if (playerRef != null)
        {
            print("Drop ball");
            taken = false;
            playerRef.hasBall = false;

            // No need to revert the player's tag because knocked out
            // playerRef.photonView.RPC("ChangePlayerTag", Photon.Pun.RpcTarget.AllBuffered, "Player");

            // Break off references
            playerRef.discoBall = null;
            playerRef = null;
            transform.parent = null;

            boxCollider.isTrigger = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sheep enters the Sphere
        if(other.CompareTag("FlockAgent")){
            print("Got sheep");
            FlockAgent sheep = other.GetComponent<FlockAgent>();
            sheep.SetDiscoBallRef(this, true);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Player Picks up the disco Ball
        if (collision.collider.CompareTag("PlayerCollider") && !taken )
        {
            print("Collected");
            PlayerController player = collision.collider.GetComponentInParent<PlayerController>();
            photonView.RPC("PickupBall", RpcTarget.AllBuffered, player.photonView.ViewID);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // Sheep enters the Sphere
        if (other.CompareTag("FlockAgent"))
        {
            print("Sheep out");
            FlockAgent sheep = other.GetComponent<FlockAgent>();
            sheep.SetDiscoBallRef(this, false);
        }
    }

    void OnDrawGizmosSelected()
    {
        // For spherecast debugging
        Gizmos.color = new Color(255.0f, 255.0f, 0.0f, 0.2f);

        Gizmos.color = Color.green;
    }

    IEnumerator PickupScan()
    {
        yield return new WaitForSeconds(0.1f);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);
        foreach (Collider col in hitColliders)
        {

            if (col.CompareTag("FlockAgent"))
            {
                print("Hit sheep");
                col.gameObject.GetComponent<FlockAgent>().SetDiscoBallRef(this, true);
            }
        }
        yield return null;
    }

    [PunRPC]
    public void DropBall()
    {
        DropBallfunc();
    }

    [PunRPC]
    public void PickupBall(int playerViewID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject i in players)
        {
            PlayerController playerctrl = i.GetComponent<PlayerController>();
            if (i.GetComponent<PhotonView>().ViewID == playerViewID)
            {
                // Setting the reference and shit

                playerRef = playerctrl;
                taken = true;
                playerctrl.discoBall = this;
                playerctrl.hasBall = true;
                // transform.SetParent(playerctrl.transform);
                transform.localPosition = snapOffset;
                boxCollider.isTrigger = true;
            }
        }

        StartCoroutine(PickupScan());
    }
}
