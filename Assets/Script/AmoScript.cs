using UnityEngine;

public class AmoScript : MonoBehaviour
{
    [Header("Réglages")]
    public float vitesse = 12f;
    public float tempsDeVie = 3f;

    private Rigidbody2D rb;
    private int degats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, tempsDeVie);
    }

    public void ConfigurerDegats(int montantDegats, Vector2 direction)
    {
        degats = montantDegats;

        rb.linearVelocity = direction * vitesse;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnnemiHealth ennemiHealthScript = collision.GetComponent<EnnemiHealth>();
            Debug.Log("Ennemi touché ! Dégâts infligés : " + degats);
            if(ennemiHealthScript != null) ennemiHealthScript.TakeDamage(degats);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}