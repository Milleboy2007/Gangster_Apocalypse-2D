using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnnemiHealth : MonoBehaviour
{
    [Header("Life")]
    [SerializeField, Min(1)]
    private int maxHealth = 100;
    private int currentHealth;
    [SerializeField]
    private GameObject backroundLife;
    [SerializeField]
    private Image fillLife;

    [Header("Regem")]
    [SerializeField, Min(0f)]
    private float delayBeforeRegen = 3f;
    [SerializeField, Min(0f)]
    private int pvPerSec = 5;
    private float timeLastDamage;
    private float regenTimer;
    private Animator animator;
    private bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if(backroundLife != null) backroundLife.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead || currentHealth >= maxHealth) return;

        if(Time.time >= timeLastDamage + delayBeforeRegen)
        {
            regenTimer += Time.deltaTime;
            float timeForOnePv = 1f / pvPerSec;

            if(regenTimer >= timeForOnePv)
            {
                currentHealth += 1;
                regenTimer = 0f;
                if(currentHealth >= maxHealth) currentHealth = maxHealth;

                UpdateSlider();
            }
        }
    }

    public void TakeDamage(int degatAmount)
    {
        currentHealth -= degatAmount;

        timeLastDamage = Time.time;
        regenTimer = 0f;

        if(currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }else animator.SetTrigger("TakeDamage");

        UpdateSlider();

    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        if(backroundLife != null) backroundLife.SetActive(false);

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<EnnemiMobile>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    void UpdateSlider()
    {
        if(fillLife != null) fillLife.fillAmount = (float)currentHealth / maxHealth;

        if(backroundLife != null) backroundLife.SetActive(currentHealth < maxHealth && currentHealth > 0);
    }
}
