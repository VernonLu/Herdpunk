using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockAgent : MonoBehaviour { 

    [Header("Movement")]
    [Range(0, 15.0f)]
    public float alertSpeed;
    [Range(0, 15.0f)]
    public float calmSpeed;

    private float speed;

    [Header("Trigger Circle")]
    public float alertRadius;
    public LayerMask castLayer;
    public LayerMask castBoundLayer;
    private RaycastHit[] results;
    private Vector3 targetDirection, lastDirection, newDestination;
    private NavMeshAgent agent;
    public float angle;

    [Header("Disco")]
    [Range(0, 15.0f)]
    public float discoAttractionSpeed;
    private DiscoBallController discoBall;

    public float stopSmoothTime;
    private List<PlayerController> playerList = new List<PlayerController>();
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        bool nearbyPlayer = AlertCheckSurrounding();
        // No player nearby and disco nearby
        if (discoBall != null)
        {
            newDestination = discoBall.transform.position;
            agent.speed = discoAttractionSpeed;
            agent.SetDestination(newDestination);
        }


        //no player nearby or disco not affecting the sheep
        else if(!nearbyPlayer && discoBall == null) {

            agent.speed = Mathf.Lerp(agent.speed, 0, stopSmoothTime);
            newDestination = transform.position + targetDirection;
            agent.SetDestination(newDestination);
        }
        // If there is a player nearby or disco not affecting the sheep
        else if (nearbyPlayer || discoBall == null)
        {
            newDestination = transform.position + targetDirection;
            if (nearbyPlayer)
            {
                agent.speed = speed;
            }
            else { agent.speed = calmSpeed;  }
            agent.SetDestination(newDestination);
        }

        

        // used to be discoball
        else
        {
            // Debug.LogWarning(nearbyPlayer);
            

        }
    }

    public bool AlertCheckSurrounding()
    {
        // Calculate the offset to the origin of the raycast
        bool result = false;
        Vector3 offset = Vector3.back;
        results = Physics.SphereCastAll(transform.position, alertRadius, Vector3.forward * 0.01f, alertRadius, castLayer);

        // Only alerted if there are player nearby
        if (results.Length != 0)
        {
            speed = alertSpeed;
            bool isFirstHit = true;
            Vector3 temp1 = Vector3.zero;
            Vector3 temp2;
            foreach (RaycastHit i in results)
            {
                // If scanned player, and the player is not holding a disco ball
                if (i.collider.CompareTag("Player") && !i.collider.GetComponent<PlayerController>().hasBall)
                {
                    result = true;
                    Debug.DrawLine(transform.position, i.point, Color.cyan);

                    // First hit just takes the direction 
                    if (isFirstHit)
                    {
                        // temp1 = transform.position - i.collider.transform.position;
                        temp1 = transform.position - i.collider.transform.position;
                        temp1 = temp1.normalized;
                        isFirstHit = false;
                    }

                    // Multiple hits, take the the average of all
                    else
                    {
                        // print("Multi hit");
                        //temp2 = transform.position - i.collider.transform.position;
                        temp2 = transform.position - i.collider.transform.position;
                        temp2 = temp2.normalized;
                        // take the middle direction
                        temp1 = Vector3.Lerp(temp1, temp2, 0.5f).normalized;
                    }
                }

                // Seperate this condition becaus sphere cast
                else if (i.collider.CompareTag("Bound"))
                {
                    if (isFirstHit)
                    {
                        // temp1 = transform.position - i.collider.transform.position;
                        temp1 = i.point - transform.position;
                        temp1 = temp1.normalized;
                        isFirstHit = false;
                    }

                    // Multiple hits, take the the average of all
                    else
                    {
                        // print("Multi hit");
                        //temp2 = transform.position - i.collider.transform.position;
                        temp2 = i.point - transform.position;
                        temp2 = temp2.normalized;
                        // take the middle direction
                        temp1 = Vector3.Lerp(temp1, temp2, 0.5f).normalized;
                    }
                }
            }
            targetDirection = temp1;
            lastDirection = targetDirection;
            Debug.DrawLine(transform.position, transform.position + targetDirection, Color.red);
        }

        /*
        // Goes to the discoBall
        else if(discoBall != null)
        {
            print("DiscoBall");
            targetDirection = discoBall.transform.position - transform.position;
            lastDirection = targetDirection;
            speed = discoAttractionSpeed;
        }
        */

        // No player nearby and no bound nearby
        else
        {
            // targetDirection = Vector3.zero;
            targetDirection = lastDirection;
            speed = calmSpeed;
            result = false;
        }

        return result;
    }

    public void SetDiscoBallRef(DiscoBallController input, bool setting) {
        if (setting) {
            discoBall = input;
        }
        else {
            discoBall = null;
        }
    }


    void OnDrawGizmosSelected()
    {
        // For spherecast debugging
        Gizmos.color = new Color(255.0f, 255.0f, 0.0f, 0.2f);

        Gizmos.DrawSphere(transform.position + Vector3.forward * 0.01f, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(newDestination, 2.0f);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out PlayerController player)) {
            if(playerList.IndexOf(player) != -1) { return; }
            playerList.Add(player);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out PlayerController player)) {
            if (playerList.IndexOf(player) == -1) { return; }
            playerList.Remove(player);
        }
    }
}
