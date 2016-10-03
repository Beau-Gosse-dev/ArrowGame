using Assets.GameObjects;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Mangers
{
    public class LevelDefinition
    {
        public bool IsPlayerLeftTurn;
        public float PlayerDistanceFromCenter;
        public float PlayerLeftHealth;
        public float PlayerRightHealth;
        public List<ShotArrow> ShotArrows;
        public int WallHeight;
        public float WallPosition;
        public GameState gameState;
        public GameType gameType;

        public float LastShotStartX;
        public float LastShotStartY;
        public float LastShotEndX;
        public float LastShotEndY;

        public string PlayerLeftId;
        public string PlayerLeftName;
        public string PlayerRightId;
        public string PlayerRightName;

        public bool RebuttalTextEnabled;

        public ParseObject matchParseObject;

        public void saveCurrentToServer()
        {
            matchParseObject["isPlayerLeftTurn"] = IsPlayerLeftTurn;
            matchParseObject["playerDistanceFromCenter"] = PlayerDistanceFromCenter;
            matchParseObject["playerLeftHealth"] = PlayerLeftHealth;
            matchParseObject["playerRightHealth"] = PlayerRightHealth;
            matchParseObject["wallHeight"] = WallHeight;
            matchParseObject["wallPosition"] = WallPosition;
            matchParseObject["gameState"] = gameState.ToString();
            matchParseObject["gameType"] = gameType.ToString();

            matchParseObject["LastShotStartX"] = LastShotStartX;
            matchParseObject["LastShotStartY"] = LastShotStartY;
            matchParseObject["LastShotEndX"] = LastShotEndX;
            matchParseObject["LastShotEndtY"] = LastShotEndY;

            matchParseObject["RebuttalTextEnabled"] = RebuttalTextEnabled;

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

        public void LevelDefinitionSetDefault()
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
            RebuttalTextEnabled = false;
        }

        public ParseObject getParseObject(string playerLeftId, string playerLeftName, string playerRightId, string playerRightName)
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

            newParseObject["LastShotStartX"] = LastShotStartX;
            newParseObject["LastShotStartY"] = LastShotStartY;
            newParseObject["LastShotEndX"] = LastShotEndX;
            newParseObject["LastShotEndtY"] = LastShotEndY;

            newParseObject["playerLeftId"] = playerLeftId;
            newParseObject["playerRightId"] = playerRightId;
            newParseObject["playerLeftName"] = playerLeftName;
            newParseObject["playerRightName"] = playerRightName;

            newParseObject["RebuttalTextEnabled"] = RebuttalTextEnabled;

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

        public void initializeFromParseObject(ParseObject matchObject)
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

            RebuttalTextEnabled = matchObject.Get<bool>("RebuttalTextEnabled");

            ShotArrows = JsonConvert.DeserializeObject<List<ShotArrow>>(matchObject.Get<string>("ShotArrows"));
            matchParseObject = matchObject;
            SceneManager.LoadScene("Friend");
        }
    }
}

