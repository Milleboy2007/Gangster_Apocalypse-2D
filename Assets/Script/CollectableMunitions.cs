using UnityEngine;

public class CollectableMunitions : MonoBehaviour
{
    [SerializeField]
    private int quantiteDonnee = 5;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAttack scriptAttaque = collision.GetComponent<PlayerAttack>();

            if (scriptAttaque != null)
            {
                if (scriptAttaque.GetStateMunition())
                {
                    scriptAttaque.AddMunition(quantiteDonnee);
                    AudioJeu.instance.JouerCollect();
                    Destroy(gameObject);
                }
                else Debug.Log("Munition maximume atteind");
            }
        }
    }
}