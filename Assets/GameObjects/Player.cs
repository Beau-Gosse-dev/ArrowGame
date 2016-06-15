using UnityEngine;
using System.Collections;

// Left shooter goes first, so it has different behavior from right shooter
public class Player : MonoBehaviour {

    public RectTransform redBar;
    public LevelManager levelManager;
    public bool isLeft;
    
    public void Hit(float damagePercent)
    {
        if (isLeft)
        {
            LevelDefinition.PlayerLeftHealth -= damagePercent;
            LevelDefinition.PlayerLeftHealth = Mathf.Clamp(LevelDefinition.PlayerLeftHealth, 0, 100);
            if (LevelDefinition.PlayerLeftHealth <= 0)
            {
                // If we are in the rebuttle stage and Left player also died, it's a tie, reset
                if (LevelDefinition.RebuttleTextEnabled)
                {
                    levelManager.EndGame(LevelManager.EndGameState.Tie);
                }
                else
                {
                    // If the left player died (who went first) it's over
                    // Mark the point
                    levelManager.EndGame(LevelManager.EndGameState.RightWins);
                }
            }
            else if (LevelDefinition.RebuttleTextEnabled) // Else if left player didn't die and we were in rebuttle, left player wins.
            {
                levelManager.EndGame(LevelManager.EndGameState.LeftWins);
            }
            redBar.sizeDelta = new Vector2(LevelDefinition.PlayerLeftHealth, redBar.rect.height);
        }
        else
        {
            LevelDefinition.PlayerRightHealth -= damagePercent;
            LevelDefinition.PlayerRightHealth = Mathf.Clamp(LevelDefinition.PlayerRightHealth, 0, 100);
            if (LevelDefinition.PlayerRightHealth <= 0)
            {
                // If the right player died first, he gets a rebuttle since he went second.
                // If he was already in rebuttal, or he shot himself, it's over.
                //if (LevelManager.levelDefinition.IsPlayerLeftTurn)
                if (LevelDefinition.RebuttleTextEnabled || LevelDefinition.IsPlayerLeftTurn)
                {
                    levelManager.EndGame(LevelManager.EndGameState.LeftWins);
                }
                else
                {
                    RebuttalText.Enable();
                }
            }
            redBar.sizeDelta = new Vector2(LevelDefinition.PlayerRightHealth, redBar.rect.height);
        }
    }

    public void SetHealth(float health)
    {
        if(isLeft)
        {
            LevelDefinition.PlayerLeftHealth = health;
            redBar.sizeDelta = new Vector2(LevelDefinition.PlayerLeftHealth, redBar.rect.height);
        }
        else
        {
            LevelDefinition.PlayerRightHealth = health;
            redBar.sizeDelta = new Vector2(LevelDefinition.PlayerRightHealth, redBar.rect.height);
        }
    }
}
