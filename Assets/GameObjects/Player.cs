using UnityEngine;

// Left shooter goes first, so it has different behavior from right shooter
public class Player : MonoBehaviour {

    public RectTransform redBar;
    public LevelManagerFriend levelManager;
    public bool isLeft;
    private NetworkManager _networkManager;
    private RebuttalText _rebuttalText;

    public void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _rebuttalText = GameObject.Find("RebuttalText").GetComponent<RebuttalText>();
    }

    void Start()
    {
        SetHealth(isLeft ? _networkManager.levelDef.PlayerLeftHealth : _networkManager.levelDef.PlayerRightHealth);
    }

    public void Hit(float damagePercent)
    {
        if (isLeft)
        {
            _networkManager.levelDef.PlayerLeftHealth -= damagePercent;
            _networkManager.levelDef.PlayerLeftHealth = Mathf.Clamp(_networkManager.levelDef.PlayerLeftHealth, 0, 100);
            if (_networkManager.levelDef.PlayerLeftHealth <= 0)
            {
                // If we are in the rebuttal stage and Left player also died, it's a tie, reset
                if (_networkManager.levelDef.RebuttalTextEnabled)
                {
                    levelManager.EndGame(LevelManagerFriend.EndGameState.Tie);
                }
                else
                {
                    // If the left player died (who went first) it's over
                    // Mark the point
                    levelManager.EndGame(LevelManagerFriend.EndGameState.RightWins);
                }
            }
            else if (_networkManager.levelDef.RebuttalTextEnabled) // Else if left player didn't die and we were in rebuttal, left player wins.
            {
                levelManager.EndGame(LevelManagerFriend.EndGameState.LeftWins);
            }
            redBar.sizeDelta = new Vector2(_networkManager.levelDef.PlayerLeftHealth, redBar.rect.height);
        }
        else
        {
            _networkManager.levelDef.PlayerRightHealth -= damagePercent;
            _networkManager.levelDef.PlayerRightHealth = Mathf.Clamp(_networkManager.levelDef.PlayerRightHealth, 0, 100);
            if (_networkManager.levelDef.PlayerRightHealth <= 0)
            {
                // If the right player died first, he gets a rebuttal since he went second.
                // If he was already in rebuttal, or he shot himself, it's over.
                //if (LevelManager.levelDefinition.IsPlayerLeftTurn)
                if (_networkManager.levelDef.RebuttalTextEnabled || _networkManager.levelDef.IsPlayerLeftTurn)
                {
                    levelManager.EndGame(LevelManagerFriend.EndGameState.LeftWins);
                }
                else
                {
                    _rebuttalText.Enable();
                }
            }
            redBar.sizeDelta = new Vector2(_networkManager.levelDef.PlayerRightHealth, redBar.rect.height);
        }
    }

    public void SetHealth(float health)
    {
        if(isLeft)
        {
            _networkManager.levelDef.PlayerLeftHealth = health;
            redBar.sizeDelta = new Vector2(_networkManager.levelDef.PlayerLeftHealth, redBar.rect.height);
        }
        else
        {
            _networkManager.levelDef.PlayerRightHealth = health;
            redBar.sizeDelta = new Vector2(_networkManager.levelDef.PlayerRightHealth, redBar.rect.height);
        }
    }
}
