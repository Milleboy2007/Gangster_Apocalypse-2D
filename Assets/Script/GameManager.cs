using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Quest Object")]
    public bool haveCarKey = false;
    public bool haveGaz = false;
    public bool haveKey = false;
    [SerializeField]
    private GameObject porte;

    void Update()
    {
        if(haveKey) porte.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
    }

}
