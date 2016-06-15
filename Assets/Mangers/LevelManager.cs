using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.IO;
using Newtonsoft.Json;
using System;

public class LevelManager : MonoBehaviour
{
    public Player playerLeft;
    public Player playerRight;
    public Text endGameText;
    public Arrow arrow;
    public Canvas endGameCanvas;
    public Score score;
    public Canvas worldCanvas;
    public AimLine aimLine;
    public RawImage fader;

    private Color replayColor = new Color(0, 0, 0, 0.5f);
    private Color playingColor = new Color(0, 0, 0, 0f);


    // For creating the wall
    public GameObject Brick;

    public const float MaxPlayerDistance = 45.0f;
    public const float MinPlayerDistance = 5.0f;

    public List<GameObject> bricks = new List<GameObject>();
    
    public void OnLevelWasLoaded()
    {
        startPlaying();
    }

    // Use this for initialization
    void Start()
    {
        startPlaying(); // TODO remove this, it is just for easy testing in the unity editor
    }

    void Update()
    {
        if (LevelDefinition.gameState == GameState.ShowLastMove)
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
        LevelDefinition.gameState = GameState.GameOver;
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
        // Set the game state to show the last player's move
        if (LevelDefinition.ShotArrows.Count > 0)
        {
            LevelDefinition.gameState = GameState.ShowLastMove;
        }
        else
        {
            LevelDefinition.gameState = GameState.Playing;
        }

        // Remove the old wall and set up the current ones.
        RemoveWalls();
        AddWalls(LevelDefinition.WallPosition, LevelDefinition.WallHeight);

        // Set the player distance
        SetPlayerWidth(LevelDefinition.PlayerDistanceFromCenter);

        // Reset the aimline (including the computer's new aim start point)
        aimLine.SetupMatch();

        // Set the world canvas (instructions above the left player)
        //worldCanvas.transform.position = new Vector3(playerLeft.transform.position.x + 6.0f, worldCanvas.transform.position.y, worldCanvas.transform.position.z);
     
        // Remove all the shot arrows,
        // Add the current ones
        // Then set the arrow's position
        arrow.RemoveAllShotArrows();
        arrow.AddShotArrows(LevelDefinition.ShotArrows);
        arrow.ResetPosition(LevelDefinition.IsPlayerLeftTurn);

        // Set the player's healths
        playerLeft.SetHealth(LevelDefinition.PlayerLeftHealth);
        playerRight.SetHealth(LevelDefinition.PlayerRightHealth);

        // Check if the game is already over.
        // Setup the rebuttle text
        RebuttalText.setEnabled(LevelDefinition.RebuttleTextEnabled);
        if (LevelDefinition.PlayerLeftHealth <= 0 && LevelDefinition.PlayerRightHealth <= 0)
        {
            EndGame(EndGameState.Tie);
        }
        else if(LevelDefinition.PlayerLeftHealth <= 0)
        {
            EndGame(EndGameState.RightWins);
        }
        else if (LevelDefinition.PlayerRightHealth <= 0 && !LevelDefinition.RebuttleTextEnabled)
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
        Application.LoadLevel("StartMenu");
    }
    public void quitPlayingLevel()
    {
        Application.LoadLevel("StartMenu");
    }

    public void SetPlayerWidth(float distance)
    {
        //float distanceFromCenter = (Random.value * MaxPlayerDistance) + MinPlayerDistance;
        playerLeft.transform.position = new Vector3(-distance, playerLeft.transform.position.y, playerLeft.transform.position.z);
        playerRight.transform.position = new Vector3(distance, playerRight.transform.position.y, playerRight.transform.position.z);
    }

    public void startNextGame()
    {
        throw new NotImplementedException();
    }
}

public static class LevelDefinition
{
    public static bool IsPlayerLeftTurn;
    public static float PlayerDistanceFromCenter;
    public static float PlayerLeftHealth;
    public static float PlayerRightHealth;
    public static List<ShotArrow> ShotArrows;
    public static int WallHeight;
    public static float WallPosition;
    public static GameState gameState;
    public static GameType gameType;

    public static float LastShotStartX;
    public static float LastShotStartY;
    public static float LastShotEndX;
    public static float LastShotEndY;

    public static string PlayerLeftId;
    public static string PlayerLeftName;
    public static string PlayerRightId;
    public static string PlayerRightName;

    public static bool RebuttleTextEnabled;

    public static ParseObject matchParseObject;

    public static void saveCurrentToServer()
    {
        matchParseObject["isPlayerLeftTurn"] = IsPlayerLeftTurn;
        matchParseObject["playerDistanceFromCenter"] = PlayerDistanceFromCenter;
        matchParseObject["playerLeftHealth"] = PlayerLeftHealth;
        matchParseObject["playerRightHealth"] = PlayerRightHealth;
        matchParseObject["wallHeight"] = WallHeight;
        matchParseObject["wallPosition"] = WallPosition;
        matchParseObject["gameState"] = gameState.ToString();
        matchParseObject["gameType"] = gameType.ToString();

        matchParseObject["LastShotStartX"] = LevelDefinition.LastShotStartX;
        matchParseObject["LastShotStartY"] = LevelDefinition.LastShotStartY;
        matchParseObject["LastShotEndX"] = LevelDefinition.LastShotEndX;
        matchParseObject["LastShotEndtY"] = LevelDefinition.LastShotEndY;

        matchParseObject["RebuttleTextEnabled"] = RebuttleTextEnabled;

        matchParseObject["ShotArrows"] = JsonConvert.SerializeObject(
            ShotArrows,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );

        matchParseObject.SaveAsync().ContinueWith(t =>
        {
            Debug.Log("Canceled: " + t.IsCanceled);
            Debug.Log("IsCompleted: " + t.IsCompleted);
            Debug.Log("IsFaulted: " + t.IsFaulted);
            Debug.Log("Exception: " + t.Exception.ToString());
            Debug.Log("InnerException: " + t.Exception.InnerException.ToString());
        });
    }

    public static void LevelDefinitionSetDefault()
    {
        IsPlayerLeftTurn = true;
        PlayerDistanceFromCenter = 5.0f;
        PlayerLeftHealth = 100.0f;
        PlayerRightHealth = 100.0f;
        ShotArrows = new List<ShotArrow>();
        WallHeight = 1;
        WallPosition = 0;
        gameState = GameState.Playing;
        gameType = GameType.Online;
        RebuttleTextEnabled = false;
    }

    public static ParseObject getParseObject(string playerLeftId, string playerLeftName, string playerRightId, string playerRightName)
    {
        ParseObject newParseObject = new ParseObject("MatchTest");
        newParseObject["isPlayerLeftTurn"] = IsPlayerLeftTurn;
        newParseObject["playerDistanceFromCenter"] = PlayerDistanceFromCenter;
        newParseObject["playerLeftHealth"] = PlayerLeftHealth;
        newParseObject["playerRightHealth"] = PlayerRightHealth;
        newParseObject["wallHeight"] = WallHeight;
        newParseObject["wallPosition"] = WallPosition;
        newParseObject["gameState"] = gameState.ToString();
        newParseObject["gameType"] = gameType.ToString();

        newParseObject["LastShotStartX"] = LevelDefinition.LastShotStartX;
        newParseObject["LastShotStartY"] = LevelDefinition.LastShotStartY;
        newParseObject["LastShotEndX"] = LevelDefinition.LastShotEndX;
        newParseObject["LastShotEndtY"] = LevelDefinition.LastShotEndY;

        newParseObject["playerLeftId"] = playerLeftId;
        newParseObject["playerRightId"] = playerRightId;
        newParseObject["playerLeftName"] = playerLeftName;
        newParseObject["playerRightName"] = playerRightName;

        newParseObject["RebuttleTextEnabled"] = RebuttleTextEnabled;

        newParseObject["ShotArrows"] = JsonConvert.SerializeObject(
            ShotArrows, 
            Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );

        return newParseObject;
    }

    public static void initializeFromParseObject(ParseObject matchObject)
    {
        IsPlayerLeftTurn = matchObject.Get<bool>("isPlayerLeftTurn");
        PlayerDistanceFromCenter = matchObject.Get<float>("playerDistanceFromCenter");
        PlayerLeftHealth = matchObject.Get<float>("playerLeftHealth");
        PlayerRightHealth = matchObject.Get<float>("playerRightHealth");
        WallHeight = matchObject.Get<int>("wallHeight");
        WallPosition = matchObject.Get<float>("wallPosition");
        gameState = (GameState)Enum.Parse(typeof(GameState), matchObject.Get<string>("gameState"));
        gameType = (GameType)Enum.Parse(typeof(GameType), matchObject.Get<string>("gameType"));

         LastShotStartX = matchObject.Get<float>("LastShotStartX");
        LastShotStartY = matchObject.Get<float>("LastShotStartY");
        LastShotEndX = matchObject.Get<float>("LastShotEndX");
        LastShotEndY = matchObject.Get<float>("LastShotEndtY");
        
        PlayerLeftId = matchObject.Get<string>("playerLeftId");
        PlayerLeftName = matchObject.Get<string>("playerLeftName");
        PlayerRightId = matchObject.Get<string>("playerRightId");
        PlayerRightName = matchObject.Get<string>("playerRightName");

        RebuttleTextEnabled = matchObject.Get<bool>("RebuttleTextEnabled");

        ShotArrows = JsonConvert.DeserializeObject<List<ShotArrow>>(matchObject.Get<string>("ShotArrows"));
        matchParseObject = matchObject;
        Application.LoadLevel("Friend");
    }
}

public enum GameType
{
    Local = 0,
    Computer,
    Online
}

public enum GameState
{
    ShowLastMove = 0,
    Playing,
    GameOver
}

public class ShotArrow
{
    public float Angle;
    public float X;
    public float Y;
    //public Vector3 Position;

    public ShotArrow(float angle, float x, float y)
    {
        Angle = angle;
        X = x;
        Y = y;
    }
}
