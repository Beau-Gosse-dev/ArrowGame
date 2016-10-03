using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Parse;
using UnityEngine.SceneManagement;

public class CreateMatch : MonoBehaviour {

    public Button createMatchButton;
    public Text myUsername;
    public RectTransform userPanelContent;
    public ButtonCreateMatch buttonCreateMatchPrefab;

    public Button AddFriendButton;
    
    private List<ButtonCreateMatchContent> userButtonContentList = new List<ButtonCreateMatchContent>();
    private NetworkManager _networkManager;

    // Use this for initialization
    void Start ()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        myUsername.text = ParseUser.CurrentUser.Username;
        FindFriends();
    }
    

    private void FindFriends()
    {
        _networkManager.loadFriends((string userName, string userId) =>
        {
            ButtonCreateMatch newButton = Instantiate(buttonCreateMatchPrefab);

            newButton.iconOfFriend = null; //todo add pic
            newButton.button.GetComponentInChildren<Text>().text = "Create New with: " + userName;
            newButton.usernameOfFriend = userName;
            newButton.userIdOfFriend = userId;
            newButton.button.onClick.AddListener(newButton.createMatch);
            newButton.transform.SetParent(userPanelContent);
            newButton.transform.localScale = new Vector3(1, 1, 1);
        });
    }

    public void loadFriendSeach()
    {
        SceneManager.LoadScene("FriendSearch");
    }

    public void loadCurrentMatches()
    {
        SceneManager.LoadScene("CurrentMatches");
    }
}
