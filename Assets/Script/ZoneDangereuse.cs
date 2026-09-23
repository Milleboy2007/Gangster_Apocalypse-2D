using UnityEngine;
using System.Collections;
using TMPro;

public class ZoneDangereuse : MonoBehaviour
{
    [SerializeField] private Transform pointDepart;
    [SerializeField] private GameObject message;

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (!autre.CompareTag("Player")) return;

        if (pointDepart == null)
        {
            Debug.LogError("Le point de départ n'est pas assigné.");
            return;
        }

        StartCoroutine(TextRetour());
        autre.transform.position = pointDepart.position;

    }

    private IEnumerator TextRetour()
    {
        message.SetActive(true);
        message.GetComponent<TextMeshProUGUI>().text = "Retour à la zone de départ";

        yield return new WaitForSeconds(5);
        message.SetActive(false);

        yield return null;
    }
}
