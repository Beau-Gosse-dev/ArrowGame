using UnityEngine;
using UnityEngine.UI;

public class ButtonAddFriend : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void addFriend()
    {
        _networkManager.addFriend(userIdOfFriend);
    }
}

public class ButtonAddFriendContent
{
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;

    public ButtonAddFriendContent(string userId, string username)
    {
        userIdOfFriend = userId;
        usernameOfFriend = username;
    }
}