using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DriveCar : MonoBehaviour
{
    [Header("Moteur")]
    [SerializeField]
    private float accel = 30f;
    [SerializeField]
    private float rotaSpeed = 130f;
    [SerializeField]
    private float maxSpeed = 15f;
    
    [Header("Adhérence")]
    [SerializeField, Range(0f, 1f)] 
    private float adherence = 0.9f; // 1 = zéro dérapage, 0 = glace totale

    private Rigidbody2D rB;
    private float inputForward;
    private float inputTurn;

    void Awake()
    {
        rB = GetComponent<Rigidbody2D>();
        rB.gravityScale = 0f;
    }

    void Update()
    {
        inputForward = Input.GetAxis("Vertical");
        inputTurn = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (rB.linearVelocity.magnitude < maxSpeed)
        {
            rB.AddForce(transform.up * inputForward * accel);
        }

        float pourcSpeed = rB.linearVelocity.magnitude / maxSpeed;
        float direction = Vector2.Dot(rB.linearVelocity, transform.up) >= 0 ? 1 : -1;
        
        rB.rotation -= inputTurn * rotaSpeed * pourcSpeed * direction * Time.fixedDeltaTime;

        Vector2 forwardSpeed = transform.up * Vector2.Dot(rB.linearVelocity, transform.up);
        Vector2 sideSpeed = transform.right * Vector2.Dot(rB.linearVelocity, transform.right);
        rB.linearVelocity = forwardSpeed + sideSpeed * (1f - adherence);
    }
}