using Assets.GameObjects;
using System.Collections.Generic;

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
    }
}

