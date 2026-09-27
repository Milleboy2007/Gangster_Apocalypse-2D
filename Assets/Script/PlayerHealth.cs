using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField, Min(1)]
    private int maxHealth = 100;
    private int currentHealth;
    [SerializeField]
    private Image healthBar;
    private Animator animator;
    private EffetDegatsJoueur degatPlayerScript;

    
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateSlider();
        degatPlayerScript = GetComponent<EffetDegatsJoueur>();
    }

    
    public void TakeDamage(int degatsAmount)
    {
        if(currentHealth == 0) return;

        currentHealth -= degatsAmount;

        if(currentHealth < 0) currentHealth = 0;

        Debug.Log("Aïe ! Le joueur perd " + degatsAmount + " PV. Reste : " + currentHealth);

        if (animator != null) degatPlayerScript.DeclencherEffet();

        UpdateSlider();

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        MonoBehaviour mouvementScript = GetComponent<MovePlayer>();
        if (mouvementScript != null) mouvementScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        animator.SetTrigger("Die");
        Invoke("CallGameOver", 2.5f);
    }

    void CallGameOver() {
        FindAnyObjectByType<GameManager>().GameOver();
    }

    void UpdateSlider()
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
