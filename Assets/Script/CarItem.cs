using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarItem : MonoBehaviour
{
    public enum ItemType { Key, Gaz }
    
    [Header("Configuration")]
    public ItemType typeOfItem;

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (autre.CompareTag("Player"))
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            
            if (gameManager != null)
            {
                if (typeOfItem == ItemType.Key) 
                {
                    gameManager.haveCarKey = true;
                    Debug.Log("Clé de voiture récupérée !");
                }
                else if (typeOfItem == ItemType.Gaz) 
                {
                    gameManager.haveGaz = true;
                    Debug.Log("Bidon d'Gaz récupéré !");
                }

                Destroy(gameObject);
            }
        }
    }
}