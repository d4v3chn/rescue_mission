using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public Gawe gawe;
    public Dragon[] dragons;
    //public Cat cat; // <- after we implement the cat
    public Text scoreText;
    public Text livesText;
    public Text gameOverText;


    public int score;
    public int lives;


    void Start()
    {
        NewGame();
    }

    private void Update()
    {
        // press R to restart the game
        if (lives <= 0 && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);


        ResetEntities(); // to reset the positions of every entities on the map

        gameOverText.gameObject.SetActive(false);
    }

    

    private void NewRound()
    {
        SetScore(score + 1);
    }

    private void GameOver()
    {
        gameOverText.text = $"Game Over! Your Score: {score}\nPress 'R' to restart.";
        gameOverText.gameObject.SetActive(true);

        StopGameplay();

    }

    private void StopGameplay()
    {
        // disables all movement
        gawe.enabled = false;
        foreach (var dragon in dragons)
        {
            dragon.enabled = false;
        }
        //cat.enabled = false; // <- after we implement the cat
    }

    private void ResetEntities()
    {

        // needs a further implementation for the entities to spawn to random locations. for that first implement entities' logic

        gawe.transform.position = new Vector2(0,0);
        gawe.enabled = true;

        foreach (var dragon in dragons) {
            dragon.transform.position = new Vector2(0, 0);
            dragon.enabled = true;
        }

        /*cat.transform.position = new Vector2(0, 0);
        cat.enabled = true;*/ // <- after we implement the cat
    }

    /*public void CatchCat()
    {

        SetScore(score + 1);
        cat.ResetCat(); // <- after we implement the cat
    }*/




    private void Death()
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


    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = $"Score: {this.score}";
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = $"Lives: {this.lives}";
    }






}
