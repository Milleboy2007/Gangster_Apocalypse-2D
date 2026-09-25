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

    // Position où replacer le joueur après un contact.
    [SerializeField] private Transform pointDepartJoueur;

    // Temps minimal entre deux applications de dégâts par cet ennemi.
    [SerializeField, Min(0.1f)]
    private float delaiEntreDegats = 1.2f;

    // Références aux composants de l'ennemi.
    private Rigidbody2D corps;
    private SpriteRenderer rendu;

    // Point actuellement visé pendant la patrouille.
    private Transform ciblePatrouille;

    // Instant à partir duquel cet ennemi pourra infliger un nouveau dégât.
    private float prochainDegat;

    // Compteur utilisé pour calculer le mouvement ondulé.
    private float progressionVague;

    // Effet visuel d'attaque, s'il existe sur cet objet.
    private EffetAttaqueEnnemi effetAttaque;

    private void Awake()
    {
        // Récupère les composants présents sur le même GameObject.
        corps = GetComponent<Rigidbody2D>();
        rendu = GetComponent<SpriteRenderer>();

        // Empêche la gravité de faire tomber l'ennemi.
        corps.gravityScale = 0f;

        // Empêche les interactions physiques de le faire tourner.
        corps.freezeRotation = true;

        // Ce composant est facultatif : GetComponent retourne null
        // s'il n'est pas présent.
        effetAttaque = GetComponent<EffetAttaqueEnnemi>();
    }

    private void Start()
    {
        // Choisit B comme première destination si B est assigné.
        // Sinon, utilise A.
        // Syntaxe ternaire : condition ? valeurSiVrai : valeurSiFaux.
        ciblePatrouille = pointB != null ? pointB : pointA;
    }

    // Appelée à intervalles fixes pour gérer la physique.
    private void FixedUpdate()
    {
        // Si le gestionnaire existe et que la partie est terminée,
        // arrête le déplacement et quitte la méthode.
        if (GestionJeu.Instance != null && GestionJeu.Instance.PartieTerminee)
        {
            corps.linearVelocity = Vector2.zero;
            return;
        }

        // Le joueur est détecté si sa référence existe ET
        // si sa distance à l'ennemi ne dépasse pas le rayon de détection.
        // && évite d'évaluer joueur.position si joueur est null.
        bool joueurDetecte = joueur != null &&
            Vector2.Distance(corps.position, joueur.position) <= rayonDetection;

        // La poursuite est prioritaire sur les autres déplacements.
        if (joueurDetecte)
            PoursuivreJoueur();
        else if (typeDeplacement == TypeDeplacement.Sinusoidal)
            DeplacementSinusoidal();
        else
            DeplacementPatrouille();
    }

    private void PoursuivreJoueur()
    {
        // Calcule le vecteur allant de l'ennemi vers le joueur.
        // Le cast Vector2 conserve uniquement les coordonnées X et Y.
        //
        // normalized conserve la direction avec une longueur de 1
        // si le vecteur n'est pas nul : la vitesse ne dépend donc
        // pas de la distance au joueur.
        Vector2 direction =
            ((Vector2)joueur.position - corps.position).normalized;

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
            ciblePatrouille = ciblePatrouille == pointA ? pointB : pointA;
    }

    private void DeplacementSinusoidal()
    {
        // Ce mode nécessite les deux points.
        if (pointA == null || pointB == null) return;

        // Fait progresser la phase de l'animation à chaque pas physique.
        progressionVague += Time.fixedDeltaTime * frequenceVague;

        // Produit une valeur qui monte de 0 à 1, puis redescend à 0.
        float allerRetour = Mathf.PingPong(progressionVague, 1f);

        // Calcule une position intermédiaire entre A et B :
        // 0 = A, 0.5 = milieu, 1 = B.
        Vector2 baseTrajet =
            Vector2.Lerp(pointA.position, pointB.position, allerRetour);

        // Sin produit une oscillation entre -1 et 1.
        // Multiplier par hauteurVague règle son amplitude.
        // Vector2.up applique ce décalage sur l'axe vertical.
        Vector2 cibleVague = baseTrajet + Vector2.up *
            (Mathf.Sin(progressionVague * Mathf.PI * 2f) * hauteurVague);

        // L'ennemi se dirige vers cette cible mobile.
        // Il n'est pas placé directement sur la courbe.
        Vector2 direction = (cibleVague - corps.position).normalized;

        // Utilise une vitesse supérieure de 15 % à celle de la patrouille.
        AppliquerVitesse(direction, vitessePatrouille * 1.15f);
    }

    // Centralise le déplacement et l'orientation visuelle.
    private void AppliquerVitesse(Vector2 direction, float vitesse)
    {
        // Définit la vitesse du Rigidbody2D en unités par seconde.
        // Il ne faut pas multiplier ici par fixedDeltaTime :
        // le moteur physique se charge de faire évoluer la position.
        corps.linearVelocity = direction * vitesse;

        // Ne change l'orientation que si le déplacement horizontal
        // est suffisamment marqué, pour éviter des retournements inutiles.
        if (Mathf.Abs(direction.x) > 0.05f)
            // Retourne le sprite horizontalement lorsque l'ennemi va à gauche.
            // Suppose que le dessin d'origine regarde vers la droite.
            rendu.flipX = direction.x < 0f;
    }

    // Appelée pendant que l'autre Collider2D reste dans la zone Trigger.
    private void OnTriggerStay2D(Collider2D autre)
    {
        // Ignore le contact si :
        // - l'objet n'a pas le tag Player;
        // - OU le délai entre deux dégâts n'est pas encore écoulé.
        if (!autre.CompareTag("Player") || Time.time < prochainDegat) return;

        // Programme le prochain instant où les dégâts seront autorisés.
        prochainDegat = Time.time + delaiEntreDegats;

        // Lance l'effet visuel de l'ennemi si la référence n'est pas null.
        effetAttaque?.Declencher();

        // Recherche la caméra principale, puis déclenche sa secousse
        // si elle possède le composant SecousseCamera.
        // if (Camera.main != null)
        //     Camera.main.GetComponent<SecousseCamera>()?.Declencher();

        // Demande au gestionnaire de retirer une vie.
        GestionJeu.Instance?.PerdreVie();

        // Si un point de retour est configuré, y replace le joueur.
        if (pointDepartJoueur != null)
        {
            autre.transform.position = pointDepartJoueur.position;

            // Recherche le Rigidbody2D sur l'objet du collider touché.
            Rigidbody2D corpsJoueur = autre.GetComponent<Rigidbody2D>();

            // Annule sa vitesse pour supprimer l'élan après le retour.
            if (corpsJoueur != null)
                corpsJoueur.linearVelocity = Vector2.zero;
        }
    }

    // Dessine des repères dans la vue Scene lorsque l'objet est sélectionné.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Visualise le rayon de détection du joueur.
        Gizmos.DrawWireSphere(transform.position, rayonDetection);

        // Visualise le segment entre les deux points de patrouille.
        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.position, pointB.position);
    }
}