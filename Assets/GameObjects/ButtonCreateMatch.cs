using UnityEngine;
using UnityEngine.UI;

public class ButtonCreateMatch : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    public CreateMatch createMatchObject;

    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void createMatch()
    {
        button.enabled = false;
        _networkManager.createMatch(userIdOfFriend, usernameOfFriend, () =>
            {
                // On success
                transform.FindChild("Image");
                var children = gameObject.GetComponentsInChildren(typeof(Image));
                foreach (var child in children)
                {
                    if (child.name == "Image")
                    {
                        ((Image)child).color = new Color(0, 1, 0);
                    }
                }
            }, () =>
            {
                // On Error
                button.enabled = true;
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