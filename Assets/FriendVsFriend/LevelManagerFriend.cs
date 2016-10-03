using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Assets.Mangers;

public class LevelManagerFriend : MonoBehaviour
{
    private Player _playerLeft;
    public Player playerRight;
    public Text endGameText;
    public Arrow arrow;
    public Canvas endGameCanvas;
    public Score score;
    public AimLineFriend aimLine;
    public Canvas worldCanvas;
    public RawImage fader;

    private Color replayColor = new Color(0, 0, 0, 0.5f);
    private Color playingColor = new Color(0, 0, 0, 0f);

    // For creating the wall
    public GameObject Brick;

    public const float MaxPlayerDistance = 45.0f;
    public const float MinPlayerDistance = 5.0f;

    public List<GameObject> bricks = new List<GameObject>();
    private NetworkManager _networkManager;
    private RebuttalText _rebuttalText;
        
    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _rebuttalText = GameObject.Find("RebuttalText").GetComponent<RebuttalText>();
        _playerLeft = GameObject.Find("PlayerLeft").GetComponent<Player>();
    }

    // Use this for initialization
    void Start()
    {        
        startPlaying(); // TODO remove this, it is just for easy testing in the unity editor
    }

    void Update()
    {
        if (_networkManager.levelDef.gameState == GameState.ShowLastMove)
        {
            // Lerp the colour of the texture between itself and black.
            fader.color = Color.Lerp(fader.color, replayColor, 1.5f * Time.deltaTime);
        }
        else
        {
            fader.color = Color.Lerp(fader.color, playingColor, 1.5f * Time.deltaTime);
        }
    }

    private void AddWalls(float baseYPosition, int numBricks)
    {
        for (int y = 0; y < numBricks; y++)
        {
            bricks.Add((GameObject)Instantiate(Brick, new Vector3(0, baseYPosition + y * Brick.GetComponent<BoxCollider2D>().size.y, 0), Quaternion.identity));
        }
    }
    private void RemoveWalls()
    {
        foreach (GameObject brick in bricks)
        {
            DestroyObject(brick);
        }
    }

    public enum EndGameState
    {
        LeftWins = 0,
        RightWins,
        Tie
    }

    public void EndGame(EndGameState endGameState)
    {
        _networkManager.levelDef.gameState = GameState.GameOver;
        StartCoroutine(EndGameOverTime(endGameState, 2.0f));
    }

    IEnumerator EndGameOverTime(EndGameState endGameState, float delayInSeconds)
    {
        if (endGameState == EndGameState.LeftWins)
        {
            score.ScoreLeft++;
            endGameText.text = "Left Player Wins!";

        }
        else if (endGameState == EndGameState.RightWins)
        {
            score.ScoreRight++;
            endGameText.text = "Right Player Wins!";
        }
        else if (endGameState == EndGameState.Tie)
        {
            endGameText.text = "Tie game!";
        }

        // Show who won for X seconds
        endGameText.enabled = true;
        yield return new WaitForSeconds(delayInSeconds);

        endGameCanvas.enabled = true;
    }

    public void startPlaying()
    {
        _networkManager.levelDef.LevelDefinitionSetDefault();

        // Set the game state to show the last player's move
        if (_networkManager.levelDef.ShotArrows.Count > 0)
        {
            _networkManager.levelDef.gameState = GameState.ShowLastMove;
        }
        else
        {
            _networkManager.levelDef.gameState = GameState.Playing;
        }

        // Remove the old wall and set up the current ones.
        RemoveWalls();
        AddWalls(_networkManager.levelDef.WallPosition, _networkManager.levelDef.WallHeight);

        // Set the player distance
        SetPlayerWidth(_networkManager.levelDef.PlayerDistanceFromCenter);

        // Reset the aimline (including the computer's new aim start point)
        aimLine.SetupMatch();

        // Set the world canvas (instructions above the left player)
        //worldCanvas.transform.position = new Vector3(playerLeft.transform.position.x + 6.0f, worldCanvas.transform.position.y, worldCanvas.transform.position.z);
     
        // Remove all the shot arrows,
        // Add the current ones
        // Then set the arrow's position
        arrow.RemoveAllShotArrows();
        arrow.AddShotArrows(_networkManager.levelDef.ShotArrows);
        arrow.ResetPosition(_networkManager.levelDef.IsPlayerLeftTurn);

        // Check if the game is already over.
        // Setup the rebuttal text
        _rebuttalText.setEnabled(_networkManager.levelDef.RebuttalTextEnabled);
        if (_networkManager.levelDef.PlayerLeftHealth <= 0 && _networkManager.levelDef.PlayerRightHealth <= 0)
        {
            EndGame(EndGameState.Tie);
        }
        else if(_networkManager.levelDef.PlayerLeftHealth <= 0)
        {
            EndGame(EndGameState.RightWins);
        }
        else if (_networkManager.levelDef.PlayerRightHealth <= 0 && !_networkManager.levelDef.RebuttalTextEnabled)
        {
            EndGame(EndGameState.LeftWins);
        }
        else
        {
            // Remove the end game text
            endGameText.enabled = false;

            // Remove the endgame menu
            endGameCanvas.enabled = false;
        }
    }
    public static void quitPlaying()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void quitPlayingLevel()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void SetPlayerWidth(float distance)
    {
        //float distanceFromCenter = (Random.value * MaxPlayerDistance) + MinPlayerDistance;
        _playerLeft.transform.position = new Vector3(-distance, _playerLeft.transform.position.y, _playerLeft.transform.position.z);
        playerRight.transform.position = new Vector3(distance, playerRight.transform.position.y, playerRight.transform.position.z);
    }

    public void startNextGame()
    {
        throw new NotImplementedException();
    }
}

