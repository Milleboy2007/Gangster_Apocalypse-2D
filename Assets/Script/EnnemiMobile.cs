using UnityEngine;

// Impose la présence des composants nécessaires sur le même GameObject :
// - Rigidbody2D : déplacement physique;
// - Collider2D : détection des contacts;
// - SpriteRenderer : affichage du sprite.
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class EnnemiMobile : MonoBehaviour
{
    // Liste des modes de déplacement disponibles dans l'Inspector.
    public enum TypeDeplacement
    {
        Patrouille, // Aller-retour entre deux points.
        Sinusoidal // Déplacement vers une cible qui suit une vague.
    }

    [Header("Patrouille")]

    // Mode utilisé lorsque le joueur n'est pas détecté.
    [SerializeField]
    private TypeDeplacement typeDeplacement = TypeDeplacement.Patrouille;

    // Repères définissant les extrémités du trajet.
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    // Vitesse de patrouille en unités Unity par seconde.
    // Min limite la valeur saisie dans l'Inspector.
    [SerializeField, Min(0.1f)]
    private float vitessePatrouille = 1.8f;

    // Amplitude verticale de la vague, en unités Unity.
    [SerializeField, Min(0.1f)]
    private float hauteurVague = 1.1f;

    // Contrôle la vitesse de progression de la vague
    // et des allers-retours entre A et B.
    [SerializeField, Min(0.1f)]
    private float frequenceVague = 1.4f;

    [Header("Attente (Pauses)")]
    [SerializeField, Min(0f)] 
    private float tempsAttentePatrouille = 1.5f; // Temps d'arrêt aux points A et B
    [SerializeField, Min(0f)] 
    private float tempsAttentePerteJoueur = 2.0f; // Temps d'arrêt quand le joueur s'échappe

    [Header("Poursuite")]

    // Position du joueur à suivre.
    [SerializeField] private Transform joueur;

    // Distance maximale à laquelle l'ennemi détecte le joueur.
    [SerializeField, Min(0.5f)]
    private float rayonDetection = 3.5f;

    // Vitesse utilisée pendant la poursuite.
    [SerializeField, Min(0.1f)]
    private float vitessePoursuite = 2.8f;


    [Header("Impact")]

    // Temps minimal entre deux applications de dégâts par cet ennemi.
    [SerializeField, Min(0.1f)]
    private float delaiEntreDegats = 1.2f;
    [SerializeField]
    private int damageToGive;
    [SerializeField, Min(0.1f)]
    private float attackRange = 1.0f;
    
    [Header("Correctif")]
    [SerializeField] 
    private Vector2 rangeGap = new Vector2(0f, 0.5f);
    [SerializeField]
    private Vector2 playerDetecGap = new Vector2(0f, 0.5f);

    // Références aux composants de l'ennemi.
    private Rigidbody2D corps;
    private SpriteRenderer rendu;

    // Point actuellement visé pendant la patrouille.
    private Transform ciblePatrouille;

    // Instant à partir duquel cet ennemi pourra infliger un nouveau dégât.
    private float prochainDegat;

    // Compteur utilisé pour calculer le mouvement ondulé.
    private float progressionVague;
    private int phasePrecedente = 0;
    private Animator animator;

    private float tempsAttenteRestant;
    private bool joueurPrecedemmentDetecte;

    private void Awake()
    {
        // Récupère les composants présents sur le même GameObject.
        corps = GetComponent<Rigidbody2D>();
        rendu = GetComponent<SpriteRenderer>();

        // Empêche la gravité de faire tomber l'ennemi.
        corps.gravityScale = 0f;

        // Empêche les interactions physiques de le faire tourner.
        corps.freezeRotation = true;

    }

    private void Start()
    {
        // Choisit B comme première destination si B est assigné.
        // Sinon, utilise A.
        ciblePatrouille = pointB != null ? pointB : pointA;
        animator = GetComponent<Animator>();
    }

    // Appelée à intervalles fixes pour gérer la physique.
    private void FixedUpdate()
    {
        Vector2 centreEnnemi = corps.position + rangeGap;
        Vector2 centrePlayer = joueur != null ? (Vector2)joueur.position + playerDetecGap : Vector2.zero;

        bool joueurDetecte = joueur != null && Vector2.Distance(corps.position, joueur.position) <= rayonDetection;


        // Si le joueur vient tout juste d'échapper à la détection (il était là, mais ne l'est plus)
        if (!joueurDetecte && joueurPrecedemmentDetecte)
        {
            tempsAttenteRestant = tempsAttentePerteJoueur; // L'ennemi s'arrête et cherche
        }
        joueurPrecedemmentDetecte = joueurDetecte;

        // Gestion du chronomètre d'attente
        if (tempsAttenteRestant > 0f)
        {
            tempsAttenteRestant -= Time.fixedDeltaTime;
            AppliquerVitesse(Vector2.zero, 0f); // Force l'arrêt complet
        }
        else
        {
            if (joueurDetecte)
            {
                float distanceWithPlayer = Vector2.Distance(centreEnnemi, centrePlayer);

                if(distanceWithPlayer <= attackRange)
                {
                    AppliquerVitesse(Vector2.zero, 0f);
                    AttackPlayer();
                }else PoursuivreJoueur();
            }
            else if (typeDeplacement == TypeDeplacement.Sinusoidal)
                DeplacementSinusoidal();
            else
                DeplacementPatrouille();
        }


        bool estEnMouvement = corps.linearVelocity.sqrMagnitude > 0.01f;
        
        if (animator != null) animator.SetBool("EnMouvement", estEnMouvement);
    }

    private void PoursuivreJoueur()
    {
        // Calcule le vecteur allant de l'ennemi vers le joueur.
        // Le cast Vector2 conserve uniquement les coordonnées X et Y.
        //
        // normalized conserve la direction avec une longueur de 1
        // si le vecteur n'est pas nul : la vitesse ne dépend donc
        // pas de la distance au joueur.
        Vector2 direction = ((Vector2)joueur.position - corps.position).normalized;

        AppliquerVitesse(direction, vitessePoursuite);
    }

    private void DeplacementPatrouille()
    {
        // Sans destination, aucun nouveau déplacement n'est calculé.
        if (ciblePatrouille == null) return;

        // Calcule la direction vers le point actuellement visé.
        Vector2 direction =
            ((Vector2)ciblePatrouille.position - corps.position).normalized;

        AppliquerVitesse(direction, vitessePatrouille);

        // Quand l'ennemi arrive près de sa destination,
        // change de point cible pour effectuer un aller-retour.
        if (Vector2.Distance(corps.position, ciblePatrouille.position) < 0.2f)
        {
                ciblePatrouille = ciblePatrouille == pointA ? pointB : pointA;
                tempsAttenteRestant = tempsAttentePatrouille;
        }
    }

    private void DeplacementSinusoidal()
    {
        if (pointA == null || pointB == null) return;

        // 1. Synchronisation parfaite : on calcule le % du trajet parcouru par seconde
        float distanceTotale = Vector2.Distance(pointA.position, pointB.position);
        float vitessePourcentage = vitessePatrouille / distanceTotale;

        // La cible fantôme avance au rythme exact de l'ennemie
        progressionVague += Time.fixedDeltaTime * vitessePourcentage;

        int phaseActuelle = Mathf.FloorToInt(progressionVague);
        
        // 2. Pause précise aux extrémités
        if (phaseActuelle > phasePrecedente)
        {
            progressionVague = phaseActuelle; // On recadre parfaitement sur le point A ou B
            phasePrecedente = phaseActuelle;
            tempsAttenteRestant = tempsAttentePatrouille; // L'ennemie prend sa pause
            return; 
        }

        // 3. Calcul du tracé (allerRetour oscille doucement entre 0 et 1)
        float allerRetour = Mathf.PingPong(progressionVague, 1f);
        Vector2 baseTrajet = Vector2.Lerp(pointA.position, pointB.position, allerRetour);
        
        // On applique les zigzags par-dessus ce tracé
        Vector2 cibleVague = baseTrajet + Vector2.up * 
            (Mathf.Sin(allerRetour * Mathf.PI * 2f * frequenceVague) * hauteurVague);

        // 4. Déplacement physique
        Vector2 direction = (cibleVague - corps.position).normalized;
        // On donne un petit boost (+25%) pour que l'ennemie reste toujours bien collé à la courbe
        AppliquerVitesse(direction, vitessePatrouille * 1.25f);
    }

    // Centralise le déplacement et l'orientation visuelle.
    private void AppliquerVitesse(Vector2 direction, float vitesse)
    {
        // Définit la vitesse du Rigidbody2D en unités par seconde.
        // Il ne faut pas multiplier ici par fixedDeltaTime :
        // le moteur physique se charge de faire évoluer la position.
        corps.linearVelocity = direction * vitesse;

        // // Ne change l'orientation que si le déplacement horizontal
        // // est suffisamment marqué, pour éviter des retournements inutiles.
        if (Mathf.Abs(direction.x) > 0.05f)
            // Retourne le sprite horizontalement lorsque l'ennemi va à gauche.
            // Suppose que le dessin d'origine regarde vers la droite.
            rendu.flipX = direction.x < 0f;
    }

    // Appelée pendant que l'autre Collider2D reste dans la zone Trigger.
    private void AttackPlayer()
    {
        Vector2 centreEnnemi = (Vector2)corps.position + rangeGap;
        Vector2 centreJoueur = (Vector2)joueur.position + playerDetecGap;
        // Ignore si :
        // - le délai entre deux dégâts n'est pas encore écoulé.
        if (Time.time < prochainDegat) return;

        // Programme le prochain instant où les dégâts seront autorisés.
        prochainDegat = Time.time + delaiEntreDegats;

        GetComponent<AudioSource>().Play();
        // Lance l'effet visuel de l'ennemi si la référence n'est pas null.
        if(animator != null) animator.SetTrigger("Attack");

        Vector2 direction = (centreJoueur - centreEnnemi).normalized;
        if (Mathf.Abs(direction.x) > 0.05f)
            rendu.flipX = direction.x < 0f;

        // Faire perdre de la vie
        joueur.GetComponent<PlayerHealth>().TakeDamage(damageToGive);

    }

    // Dessine des repères dans la vue Scene lorsque l'objet est sélectionné.
    private void OnDrawGizmosSelected()
    {
        Vector2 centreVisuel = (Vector2)transform.position + rangeGap;
        Gizmos.color = Color.yellow;

        // Visualise le rayon de détection du joueur.
        Gizmos.DrawWireSphere(centreVisuel, rayonDetection);

        // Visualise le rayon d'attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centreVisuel, attackRange);

        // Visualise le segment entre les deux points de patrouille.
        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.position, pointB.position);
    }
}