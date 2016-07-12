using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Mangers
{
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
            SceneManager.LoadScene("Friend");
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
}

