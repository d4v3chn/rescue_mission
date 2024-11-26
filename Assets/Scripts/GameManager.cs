using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Gawe gawe;
    public Dragon[] dragons;
    public Text scoreText;
    public Text livesText;
    public Text gameOverText;

    public int score;
    public int lives;

    private void Awake()
    {

        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        livesText = GameObject.Find("LivesText")?.GetComponent<Text>();
        gameOverText = GameObject.Find("GameOverText")?.GetComponent<Text>();


        if (scoreText == null || livesText == null || gameOverText == null)
        {
            Debug.LogError("GameManager is missing required UI elements. Please ensure they are properly set up in the scene.");
            enabled = false;
            return;
        }

        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        ResetEntities();
        gameOverText.gameObject.SetActive(false);
    }

    private void ResetEntities()
    {

        gawe.ResetState();


        foreach (var dragon in dragons)
        {
            dragon.transform.position = GetRandomPosition();
            dragon.enabled = true;
        }
    }

    public void Death()
    {
        SetLives(lives - 1);

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            ResetEntities();
        }
    }

    private void GameOver()
    {
        gameOverText.text = $"Game Over! Your Score: {score}\nPress 'R' to restart.";
        gameOverText.gameObject.SetActive(true);

        StopGameplay();
    }

    private void StopGameplay()
    {
        gawe.enabled = false;
        foreach (var dragon in dragons)
        {
            dragon.enabled = false;
        }
    }

    private void SetScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"Score: {score}";
    }

    private void SetLives(int newLives)
    {
        lives = newLives;
        livesText.text = $"Lives: {lives}";
    }

    private Vector2 GetRandomPosition()
    {
        
        return new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }
}