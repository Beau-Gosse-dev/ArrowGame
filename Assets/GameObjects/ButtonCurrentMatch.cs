using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using Assets.Managers;
using Assets.Mangers;

public class ButtonCurrentMatch : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public string matchId;
    public float playerLeftHealth;
    public float playerRightHealth;
    
    public void LoadMatch()
    {
        ParseQuery<ParseObject> query = ParseObject.GetQuery("MatchTest");
        query.GetAsync(this.matchId).ContinueWith(t =>
        {
            ParseObject match = t.Result;
            NetworkManager.Call(() =>
            {
                LevelDefinition.initializeFromParseObject(match);
            });
        });
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