using UnityEngine;

public class CollectableLife : MonoBehaviour
{
    [SerializeField]
    private int healtAmount = 25;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth scriptHealth = collision.GetComponent<PlayerHealth>();

            if (scriptHealth != null)
            {
                if (scriptHealth.GetStateHealth())
                {
                    scriptHealth.AddHealth(healtAmount);
                    AudioJeu.instance.JouerCollect();
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Vie déjà au maximum, pack laissé au sol.");
                }
            }
        }
    }
}