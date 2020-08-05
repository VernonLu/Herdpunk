using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun {
    [Header("Movement")]
    [Range(0, 15.0f)]
    public float defaultSpeed;
    private float speed;
    [Range(0, 10.0f)]
    public float turnSpeed;
    public bool canMove;

    [Header("Dashing")]
    [Range(0, 150.0f)]
    public float dashSpeed;
    public float dashDistance;
    public float dashDuration;
    public float dashCD;
    private float dashTimer;
    private bool canDash = true;
    private bool dashing = false;
    private bool interupted = false;

    [Header("Knockout")]
    public float knockOutTime;

    [Header("DiscoBall")]
    public DiscoBallController discoBall;
    public bool hasBall = false;

    [Header("UI elements")]
    public PlayerUIController playerUI;
    /*public Image dashCD_img;
    public Image knockout;
    public Text nametag;
    */
    public Transform dashCD_Pos, knock_Pos, nametag_Pos;


    private Vector3 lastDirection;
    private Vector3 input;
    private Rigidbody rb;
    private float angle;
    private IEnumerator dash;

    public List<Renderer> playerRenderers;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dashClip;
    public AudioClip stunClip;


    [Header("VFX")]
    public GameObject dashEffect;
    public GameObject stunEffect;
    void Start() {
        rb = GetComponent<Rigidbody>();
        dashTimer = dashCD;
        speed = defaultSpeed;
        dash = Dash();
    }

    void Update() {
        if (photonView.IsMine || !PhotonNetwork.IsConnected) {
            if (canMove) {
                Movement();
            }
            UpdateUI();
            UpdateCD();
        }
    }

    private void UpdateUI() {
        playerUI.dashCD_img.fillAmount = Mathf.Clamp((dashTimer / dashCD), 0.0f, 1.0f);
    }

    private void UpdateCD()
    {
        dashTimer += Time.deltaTime;
        if(dashTimer >= dashCD)
        {
            canDash = true;
        }
    }

    private void Movement() {
        GetInput();

        // Mousebutton OR Key "J" triggers dash
        // TODO: Input manager
        if (canDash)
        {
            if (Input.GetButtonDown("Jump"))
            {
                print("Dash");
                StartCoroutine(Dash());
                dashTimer = 0.0f;
                canDash = false;
            }
        }

        rb.velocity = lastDirection * speed;
    }

    private void GetInput() {
        float x, y;
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        input = new Vector3(x, 0.0f, y);

        // No Input Detected
        if (Mathf.Abs(x) < 0.02f && Mathf.Abs(y) < 0.02f) {
            lastDirection = new Vector3(0, 0, 0);
        }

        else {
            CalculateDirection();
            Rotate();
            lastDirection = transform.forward;
        }
    }

    void CalculateDirection() {
        angle = Mathf.Atan2(input.x, input.z);
        angle *= Mathf.Rad2Deg;
    }

    void Rotate() {
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dashing)
        {
            // Ram into sheep or wall

            if (other.CompareTag("FlockAgent") || other.CompareTag("Bound"))
            {
                Debug.LogWarning("Run into bound or sheep: " + other.gameObject.name);

                interupted = true;
                dashing = false;
                
                StartCoroutine(Knockout());
            }

            // Ram into other players
            else if (other.CompareTag("Player"))
            {
                print("Dash into another player");
                interupted = true;
                dashing = false;
                other.gameObject.GetComponent<PhotonView>().RPC("KnockedOut", RpcTarget.AllBuffered);
                canMove = true;
                interupted = false;
            }
        }
    }

    IEnumerator Dash()
    {
        // Dash to destination in dashDuration
        // Bug: doesn't take exactly dashDuration, always dashing at a constantish speed
        float elapsedTime = 0.0f;

        //Play dash audio
        audioSource.PlayOneShot(dashClip);
        photonView.RPC("ToggleDashVisual", RpcTarget.AllBuffered, true);
        

        while (elapsedTime <= dashDuration)
        {
            if (interupted)
            {
                speed = defaultSpeed;
                yield break;
            }
            canMove = false;
            dashing = true;
            lastDirection = transform.forward;
            speed = dashSpeed;
            // transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canMove = true;
        dashing = false;
        speed = defaultSpeed;
        yield return null;
    }
    IEnumerator Knockout()
    {
        print("Knocked out");
        // Placeholder until the VFX for knockout is done
        playerUI.photonView.RPC("ToggleKnockoutVisual", RpcTarget.AllBuffered, true);
        //
        photonView.RPC("ToggleStunVisual", RpcTarget.AllBuffered);

        //Play knockout audio
        audioSource.PlayOneShot(stunClip);

        // Stop the player
        rb.velocity = Vector3.zero;
        lastDirection = Vector3.zero;
        canMove = false;
        photonView.RPC("ChangePlayerTag", RpcTarget.AllBuffered, "Untagged");

        yield return new WaitForSeconds(knockOutTime);
        
        // Resume
        canMove = true;
        interupted = false;

        photonView.RPC("ChangePlayerTag", RpcTarget.AllBuffered, "Player");

        playerUI.photonView.RPC("ToggleKnockoutVisual", RpcTarget.AllBuffered, false);
    }


    [PunRPC]
    public void SetPlayerUIRef(int uiViewID)
    {
        GameObject[] playerUIs = GameObject.FindGameObjectsWithTag("PlayerUI");
        foreach (GameObject i in playerUIs)
        {
            PlayerUIController playerUIctrl = i.GetComponent<PlayerUIController>();
            if (i.GetComponent<PhotonView>().ViewID == uiViewID)
            {
                playerUI = playerUIctrl;
            }
        }
    }
    [PunRPC]
    public void SetPlayerColor(int viewID, int team) {
        if(viewID == photonView.ViewID) {
            foreach( var render in playerRenderers) {
                render.material.SetColor("_EmissionColor", PlayerSpawner.Instance.GetColor(team));
            }
        }
    }

    [PunRPC]
    public void KnockedOut()
    {
        StartCoroutine(Knockout());
        if(discoBall != null)
        {
            // discoBall.DropBall();
            discoBall.photonView.RPC("DropBall", RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// [RPC]Give player control access.
    /// player can move characters now
    /// </summary>
    [PunRPC]
    public void GrantControl() {
        if (photonView.IsMine) {
            Debug.Log("You can Control your character now!");
            canMove = true;
        }
    }

    /// <summary>
    /// Retrieve player control access.
    /// player can not move character
    /// </summary>
    [PunRPC]
    public void RetrieveControl() {
        if (photonView.IsMine) {
            Debug.Log("You can't Control your character now!");
            canMove = false;
        }
    }

    [PunRPC]
    public void ChangePlayerTag(string tagInput) {
        this.gameObject.tag = tagInput;
    }


    [PunRPC]
    public void ToggleDashVisual(bool activate) {
        // Toggle on
        if (activate) {
            if (photonView.IsMine) {
                PhotonNetwork.Instantiate(dashEffect.name, transform.position + new Vector3(0, 2.9f, 0), transform.rotation);
            }
        }
    }

    [PunRPC]
    public void ToggleStunVisual() {
        if (photonView.IsMine) {
            PhotonNetwork.Instantiate(stunEffect.name, transform.forward + transform.position + new Vector3(0, 3f, 0), transform.rotation);
        }
    }

}