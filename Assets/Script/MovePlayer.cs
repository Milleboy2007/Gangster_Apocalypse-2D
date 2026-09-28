using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MovePlayer : MonoBehaviour
{
    [SerializeField] private float vitesse = 5f;

    private Rigidbody2D corps;
    private Animator animator;
    private Vector2 direction;
    private bool commandesActives = true;

    private void Awake()
    {
        corps = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!commandesActives)
        {
            direction = Vector2.zero;
            animator.SetBool("EnMouvement", false);
            return;
        }

        direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        animator.SetBool("EnMouvement", direction.sqrMagnitude > 0.01f);
        if (direction[0] > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }else if(direction[0] < 0){
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        corps.linearVelocity = commandesActives ? direction * vitesse : Vector2.zero;
    }

    public void DesactiverCommandes()
    {
        commandesActives = false;
        direction = Vector2.zero;
        corps.linearVelocity = Vector2.zero;
        animator.SetBool("EnMouvement", false);
    }
}
