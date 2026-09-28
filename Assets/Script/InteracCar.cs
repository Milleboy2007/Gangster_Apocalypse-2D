using UnityEngine;
using TMPro;

public class VoitureInteraction : MonoBehaviour
{
    [Header("Ref Car")]
    [SerializeField] private MonoBehaviour driveScript;
    [SerializeField] private Transform exitePoint;
    private Rigidbody2D carRB;

    [Header("Ref player")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private MonoBehaviour movePlayerScript;
    [SerializeField]
    private SpriteRenderer playerVisual; 
    
    [Header("Other")]
    [SerializeField]
    private GameObject message;
    private GameManager gameManager;

    private Rigidbody2D playerRB;

    private bool isPlayerClose = false;
    private bool isInCar = false;

    void Awake()
    {
        carRB = GetComponent<Rigidbody2D>();
        carRB.bodyType = RigidbodyType2D.Kinematic; 
    }

    void Start()
    {
        if(player != null) playerRB = player.GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (isInCar) ExiteCar();
            else if (isPlayerClose) {
                if(gameManager != null && gameManager.haveCarKey && gameManager.haveGaz) EnterCar();
                else Debug.Log("Il manque un objet pour utiliser la voiture");
            }
        }

        if(isInCar && player != null) player.transform.position = transform.position;
    }

    private void EnterCar()
    {
        isInCar = true;

        if (playerRB != null) playerRB.simulated = false;
        if (playerVisual != null) playerVisual.enabled = false;
        if (movePlayerScript != null) movePlayerScript.enabled = false;
        
        driveScript.enabled = true;
        carRB.bodyType = RigidbodyType2D.Dynamic;
    }

    private void ExiteCar()
    {
        isInCar = false;

        driveScript.enabled = false;
        carRB.linearVelocity = Vector2.zero;
        carRB.angularVelocity = 0f;
        carRB.bodyType = RigidbodyType2D.Kinematic;

        player.transform.position = exitePoint.position;
        
        if (playerRB != null) playerRB.simulated = true;
        if (playerVisual != null) playerVisual.enabled = true;
        if (movePlayerScript != null) movePlayerScript.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (autre.CompareTag("Player")) isPlayerClose = true;
        if(message != null)
        {
            message.GetComponent<TextMeshProUGUI>().text = "Tap E to enter";
            message.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D autre)
    {
        if (autre.CompareTag("Player")) isPlayerClose = false;
        if(message != null) message.SetActive(false);
    }
}