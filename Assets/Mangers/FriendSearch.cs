using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FriendSearch : MonoBehaviour {
    
    public Button SearchButton;
    public Button GoToCreateMatchButton;
    public InputField UserNameField;
    public Text myUsername;
    public Transform contentPanel;
    public ButtonAddFriend buttonAddFriendPrefab;
    private NetworkManager _networkManager;

    private List<string> IdsAlreadyInList;
    private Object lockObject = new Object();

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
        IdsAlreadyInList = new List<string>();
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
            lock (lockObject)
            {
                foreach (ButtonAddFriendContent user in users)
                {
                    if (IdsAlreadyInList.Contains(user.userIdOfFriend))
                    {
                        // This user was already returned by another means, e.g. username and now display name, so just return
                        return;
                    }

                    Debug.Log("User: " + user.usernameOfFriend);
                    IdsAlreadyInList.Add(user.userIdOfFriend);

                    NetworkManager.CallOnMainThread(() =>
                    {
                        ButtonAddFriend newButton = Instantiate(buttonAddFriendPrefab);
                        newButton.iconOfFriend = null; // TODO get Facebook pictures
                        newButton.button.GetComponentInChildren<Text>().text = "Add friend: " + user.usernameOfFriend;
                        newButton.usernameOfFriend = user.usernameOfFriend;
                        newButton.userIdOfFriend = user.userIdOfFriend;
                        newButton.button.onClick.AddListener(newButton.addFriend);
                        newButton.transform.SetParent(contentPanel);
                        newButton.transform.localScale = new Vector3(1, 1, 1);
                    });
                }
            }
        });
    }
}