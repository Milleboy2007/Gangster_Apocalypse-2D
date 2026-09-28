using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Quest Object")]
    public bool haveCarKey = false;
    public bool haveGaz = false;

    public void GameOver()
    {
        Time.timeScale = 0f;
    }

}
