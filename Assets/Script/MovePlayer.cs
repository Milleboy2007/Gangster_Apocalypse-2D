using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlayer : MonoBehaviour
{
    [SerializeField] private float vitesse = 5f;

    private Rigidbody2D corps;
    private Vector2 direction;
    private Animator animator;

    private void Awake()
    {
        corps = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(horizontal, vertical).normalized;

        animator.SetFloat("Vitesse", direction.sqrMagnitude);

        if (direction[0] > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }else if(direction[0] < 0){
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        corps.MovePosition(corps.position + direction * vitesse * Time.fixedDeltaTime);
    }
}