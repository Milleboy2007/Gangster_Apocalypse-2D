using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Quest Object")]
    public bool haveCarKey = false;
    public bool haveGaz = false;
    public bool haveKey = false;
    [SerializeField]
    private GameObject porte;
    [SerializeField]
    private GameObject gameOverPanel;

    void Update()
    {
        if(haveKey) porte.SetActive(true);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
