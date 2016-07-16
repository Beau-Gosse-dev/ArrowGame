using UnityEngine;
using UnityEngine.UI;
using Parse;
using Assets.Mangers;

public class ButtonCreateMatch : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public CreateMatch createMatchObject;

    private NetworkManager _networkManager;

    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }


    public void createMatch()
    {
        LevelDefinition.LevelDefinitionSetDefault();


        ParseObject matchParseObject = LevelDefinition.getParseObject(
            ParseUser.CurrentUser.ObjectId, 
            ParseUser.CurrentUser.Username, 
            userIdOfFriend, 
            usernameOfFriend
            );

        matchParseObject.SaveAsync();
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