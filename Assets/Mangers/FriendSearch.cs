using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FriendSearch : MonoBehaviour {
    
    public Button SearchButton;
    public Button GoToCreateMatchButton;
    public InputField UserNameField;
    public Text myUsername;
    public Transform contentPanel;
    public ButtonAddFriend buttonAddFriendPrefab;
    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
    
    void Start ()
    {
        myUsername.text = _networkManager.CurrentUser.UserName;
    }

    public void GoToCreateMatch()
    {
        SceneManager.LoadScene("CreateMatch");
    }

    public void SearchUser ()
    {
        string searchString = UserNameField.text;

        _networkManager.searchFriends(searchString, users => 
            {
                foreach (ButtonAddFriendContent user in users)
                {
                    Debug.Log("User" + user.usernameOfFriend);
                    NetworkManager.Call(() =>
                    {
                        ButtonAddFriend newButton = Instantiate(buttonAddFriendPrefab);
                        newButton.iconOfFriend = null; // TODO get Facebook pictures
                        newButton.button.GetComponentInChildren<Text>().text = user.usernameOfFriend;
                        newButton.usernameOfFriend = user.usernameOfFriend;
                        newButton.userIdOfFriend = user.userIdOfFriend;
                        newButton.button.onClick.AddListener(newButton.addFriend);
                        newButton.transform.SetParent(contentPanel);
                        newButton.transform.localScale = new Vector3(1, 1, 1);
                    });
                }
            });
    }
}