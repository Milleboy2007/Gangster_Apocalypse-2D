using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PointSortie : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private string nameOtherScene;
    [SerializeField]
    private GameObject message;
    [SerializeField]
    private GameObject vic;
    private bool isPlayerClose;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerClose)
        {
            if (gameManager.haveKey)
            { 
                Debug.Log("Chargement de la scène : " + nameOtherScene);
                if(nameOtherScene.Trim() != "") SceneManager.LoadScene(nameOtherScene);
            }else Debug.Log("Key missing!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;

            if(message != null)
            {
                message.GetComponent<TextMeshProUGUI>().text = "Tap E to switch level";
                message.SetActive(true);
            }
        }

        if (other.CompareTag("Car"))
        {
            Time.timeScale = 0f;
            vic.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerClose = false;
        message.SetActive(false);
    }
}
