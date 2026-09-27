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

    
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateSlider();
    }

    
    public void TakeDamage(int degatsAmount)
    {
        if(currentHealth == 0) return;

        currentHealth -= degatsAmount;

        if(currentHealth < 0) currentHealth = 0;

        Debug.Log("Aïe ! Le joueur perd " + degatsAmount + " PV. Reste : " + currentHealth);

        if (animator != null) animator.SetTrigger("Hurt");

        UpdateSlider();

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Game END");
    }

    void UpdateSlider()
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
