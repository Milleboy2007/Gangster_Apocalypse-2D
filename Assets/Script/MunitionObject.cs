using UnityEngine;

public class MunitionObject : MonoBehaviour
{
    private static int compteur = 0;
    private HUDManager hUDManager;

    private void OnTriggerEnter2D(Collider2D autre)
    {
        // TODO 1 : vérifier que autre possède le tag Player
        // TODO 2 : augmenter compteur
        // TODO 3 : afficher la nouvelle valeur
        // TODO 4 : détruire cet objet

        if (autre.CompareTag("Player"))
        {
            compteur += 1;
            autre.GetComponent<HUDManager>().updateMunText(compteur);
            Destroy(gameObject);
        }
    }
}
