using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using Assets.Managers;

public class ButtonCreateMatch : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public CreateMatch createMatchObject;

    public void createMatch()
    {
        LevelDefinition.LevelDefinitionSetDefault();
        ParseObject matchParseObject = LevelDefinition.getParseObject(
            ParseUser.CurrentUser.ObjectId, 
            ParseUser.CurrentUser.Username, 
            userIdOfFriend, 
            usernameOfFriend
            );

        matchParseObject.SaveAsync().ContinueWith(t =>
        {
        });
    }
}

public class ButtonCreateMatchContent
{
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;

    public ButtonCreateMatchContent(string userId, string username)
    {
        userIdOfFriend = userId;
        usernameOfFriend = username;
    }
}