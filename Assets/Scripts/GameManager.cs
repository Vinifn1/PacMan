using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;

    public Pacman pacman;

    public Transform pellets;

    public TextMeshProUGUI vidas;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI victoryText;

    public int Score { get; private set; }
    public int Lives { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        vidas.text = Lives.ToString();
        // if (Lives == 0 && Input.anyKeyDown)
        // {
        //     NewGame();
        // }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMenu();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.Score + ghost.points);
        ghost.gameObject.SetActive(false);
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(Lives - 1);

        if (Lives == 0)
            GameOver();
        else
            Invoke(nameof(ResetState), 3.0f);
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        
        SetScore(this.Score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);

            victoryText.enabled = true;
            
            Invoke(nameof(GoToMenu), 3.0f);
        }
    }
    
    public void PowerPelletEaten(PowerPellet powerPellet)
    {
        foreach (var ghost in ghosts)
        {
            ghost.Frightened.Enable(powerPellet.duration);    
        }
        
        PelletEaten(powerPellet);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
                return true;
        }
        return false;
    }
    
    private void GameOver()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        
        this.pacman.gameObject.SetActive(false);

        gameOverText.enabled = true;
        
        // O Método invoke serve para esperar a quantidade de segundos, e depois executar o metodo GoToMenu
        Invoke(nameof(GoToMenu),3.0f);
    }

    private void GoToMenu()
    {
        ChangeScene.MoveToScene(0);
    }

    private void NewGame()
    {
        
        gameOverText.enabled = false;
        victoryText.enabled = false;
        SetScore(0);
        SetLives(1);
    }

    private void NewRound()
    {
        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }
    private void ResetState()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.ResetState();
        }
        
        this.pacman.ResetState();
    }

    private void SetScore(int score)
    {
        Score = score;
    }

    private void SetLives(int lives)
    {
        Lives = lives;
    }
}
