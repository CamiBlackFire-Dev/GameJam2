using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI score, cronometer;
    public float seconds, actualScore;

    void Update()
    {
        if (seconds > 0)
        {
            int timer;
            seconds -= Time.deltaTime;
            timer = Mathf.RoundToInt(seconds);
            PlayTime(timer);
        }
        
            
    }

    void PlayTime(int time)
    {
        cronometer.text = time.ToString();
        if (time <= 0)
        {
            GameOver();
        }
        if(time<= 0 && actualScore<= 0  ){
            GameOver2();
        }
    }

    public void PlayerScore()
    {
        actualScore += 1;
        score.text = actualScore.ToString();
    }

    void GameOver()
    {
        SceneManager.LoadScene("Final");
    }
    void GameOver2()
    {
        SceneManager.LoadScene("Final2");
    }
}
