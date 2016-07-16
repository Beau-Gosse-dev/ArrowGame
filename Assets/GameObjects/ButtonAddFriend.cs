using UnityEngine;
using UnityEngine.UI;

public class ButtonAddFriend : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;

    public void addFriend()
    {
        FriendSearch.addFriend(userIdOfFriend);
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