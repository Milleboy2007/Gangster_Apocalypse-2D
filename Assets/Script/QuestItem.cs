using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarItem : MonoBehaviour
{
    public enum ItemType { CarKey, Gaz, key }
    
    [Header("Configuration")]
    public ItemType typeOfItem;

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (autre.CompareTag("Player"))
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            
            if (gameManager != null)
            {
                if (typeOfItem == ItemType.CarKey) 
                {
                    gameManager.haveCarKey = true;
                    Debug.Log("Clé de voiture récupérée !");
                }else if (typeOfItem == ItemType.Gaz) 
                {
                    gameManager.haveGaz = true;
                    Debug.Log("Bidon d'Gaz récupéré !");
                }else if (typeOfItem == ItemType.key)
                {
                    gameManager.haveKey = true;
                    Debug.Log("Key récupérée!");
                }

                AudioJeu.instance.JouerCollect();
                Destroy(gameObject);
            }
        }
    }
}