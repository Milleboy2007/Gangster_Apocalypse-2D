using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EffetDegatsJoueur : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private SpriteRenderer renduPlayer;

    [Header("Interface")]
    [SerializeField]
    private CanvasGroup tachesDeSang; 

    [Header("Animation")]
    [SerializeField]
    private Color couleurDegat = Color.red;

    [SerializeField, Min(0.1f)]
    private float dureeDisparitionSang = 2.0f; // Le temps (en secondes) que met le sang à disparaître

    [SerializeField, Min(1f)]
    private float agrandissement = 1.12f;

    private Color couleurInitiale;
    private Vector3 echelleInitiale;
    private Coroutine effetEnCours;

    private void Awake()
    {
        if (renduPlayer == null)
        {
            renduPlayer = GetComponent<SpriteRenderer>();
        }

        if (renduPlayer != null)
        {
            couleurInitiale = renduPlayer.color;
        }

        echelleInitiale = transform.localScale;

        if (tachesDeSang != null)
        {
            tachesDeSang.GameObject().SetActive(true); // Activation du sang puisque désactiver pour ne pas gênée l'édition
            tachesDeSang.alpha = 0f; // Le sang est invisible au démarrage
        }
    }

    public void DeclencherEffet()
    {
        if (effetEnCours != null)
        {
            StopCoroutine(effetEnCours);
        }

        effetEnCours = StartCoroutine(JouerEffetSang());
    }

    private IEnumerator JouerEffetSang()
    {
        // 1. IMPACT : Le joueur grossit, rougit, et le sang apparaît à l'écran
        transform.localScale = echelleInitiale * agrandissement;
        if (renduPlayer != null) renduPlayer.color = couleurDegat;
        
        if (tachesDeSang != null) tachesDeSang.alpha = 1f; // Opacité à 100%

        // Petit temps d'arrêt pour marquer l'impact sur le joueur (ex: 0.15 seconde)
        yield return new WaitForSeconds(0.15f);

        // Le joueur reprend sa couleur et sa taille normale
        if (renduPlayer != null) renduPlayer.color = couleurInitiale;
        transform.localScale = echelleInitiale;

        // 2. FONDU : Le sang disparaît progressivement
        if (tachesDeSang != null)
        {
            float tempsEcoule = 0f;
            
            // On fait une boucle qui tourne tant que le temps écoulé est inférieur à la durée voulue
            while (tempsEcoule < dureeDisparitionSang)
            {
                tempsEcoule += Time.deltaTime; // Ajoute le temps passé depuis la dernière frame
                
                // Mathf.Lerp calcule une valeur fluide entre 1 (départ) et 0 (fin) en fonction du temps
                tachesDeSang.alpha = Mathf.Lerp(1f, 0f, tempsEcoule / dureeDisparitionSang);
                
                yield return null; // Attend la frame suivante avant de continuer la boucle
            }

            // Par sécurité, on s'assure qu'à la fin de la boucle, l'opacité est bien exactement à zéro
            tachesDeSang.alpha = 0f;
        }

        effetEnCours = null;
    }

    private void OnDisable()
    {
        if (renduPlayer != null) renduPlayer.color = couleurInitiale;
        if (tachesDeSang != null) tachesDeSang.alpha = 0f;
        transform.localScale = echelleInitiale;
    }
}