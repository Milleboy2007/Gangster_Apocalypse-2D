using UnityEngine;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    [Header("Combat à Distance (Clic Gauche)")]
    [SerializeField]
    private float fireRate = 0.4f;
    [SerializeField]
    private int fireDegat = 15;
    [SerializeField]
    private GameObject amoPrefab;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private int maxMunition = 20;
    [SerializeField]
    private int currentMunitions = 5;
    [SerializeField]
    private TextMeshProUGUI texteMunitions;

    [Header("Combat Rapproché (Clic Droit)")]
    [SerializeField]
    private float meleeRate = 0.6f;
    [SerializeField]
    private int meleeDegat = 10;
    [SerializeField]
    private Transform meleePoint;
    [SerializeField]
    private float attackRange = 0.5f;

    private Animator animator;
    private float timeNextHit = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateMunitionText();
    }

    void Update()
    {
        if (Time.time >= timeNextHit)
        {
            if (Input.GetMouseButtonDown(0)) // 0 = Clic Gauche
            {
                AttaqueDistance();
            }
            else if (Input.GetMouseButtonDown(1)) // 1 = Clic Droit
            {
                AttaqueMelee();
            }
        }
    }

    void AttaqueDistance()
    {
        if (currentMunitions <= 0)
        {
            Debug.Log("À court de munitions !");
            return;
        }

        if (amoPrefab == null)
        {
            Debug.LogWarning("Aucun Prefab de munition assigné !");
            return;
        }

        timeNextHit = Time.time + fireRate;

        currentMunitions--;
        UpdateMunitionText();

        if (animator != null) animator.SetTrigger("Shoot");

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotationBalle = Quaternion.Euler(new Vector3(0, 0, angle));

        // Instanciation du projectile
        GameObject newAmo = Instantiate(amoPrefab, firePoint.position, rotationBalle);
        AmoScript amoScript = newAmo.GetComponent<AmoScript>();

        if (amoScript != null)
        {
            amoScript.ConfigurerDegats(fireDegat, shootDirection);
        }
    }

    void AttaqueMelee()
    {
        timeNextHit = Time.time + meleeRate;

        if (animator != null) animator.SetTrigger("Melee");

        Collider2D[] objetsTouches = Physics2D.OverlapCircleAll(meleePoint.position, attackRange);
            
        foreach (Collider2D objet in objetsTouches)
        {
            if (objet.CompareTag("Enemy"))
            {
                EnnemiHealth ennemiHealthScript = objet.GetComponent<EnnemiHealth>();

                if (ennemiHealthScript != null)
                {
                    ennemiHealthScript.TakeDamage(meleeDegat);
                }
            }
        }
    }

    public void AddMunition(int amount) 
    {
        currentMunitions += amount;
        if (currentMunitions > maxMunition) currentMunitions = maxMunition;
        UpdateMunitionText();
    }

    void UpdateMunitionText()
    {
        if (texteMunitions != null) texteMunitions.text = currentMunitions.ToString();
    }

    public bool GetStateMunition()
    {
        return currentMunitions < maxMunition && currentMunitions >= 0;
    }

    void OnDrawGizmosSelected()
    {
        if(meleePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleePoint.position, attackRange);
        }

        if(firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePoint.position, 0.5f);
        }
    }
}