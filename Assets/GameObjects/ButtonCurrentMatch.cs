using Assets.Mangers;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCurrentMatch : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public LevelDefinition levelDefinition;
    public float playerLeftHealth;
    public float playerRightHealth;


    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void LoadMatch()
    {
        _networkManager.loadMatch(levelDefinition);
    }
}

public class ButtonCurrentMatchContent
{
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public string matchId;
    public float playerLeftHealth;
    public float playerRightHealth;

    public ButtonCurrentMatchContent(string userId, string username, string matchIdIn, float leftHealth, float rightHealth)
    {
        userIdOfFriend = userId;
        usernameOfFriend = username;
        matchId = matchIdIn;
        playerLeftHealth = leftHealth;
        playerRightHealth = rightHealth;
    }
}