using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Assets.Networking;

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
                foreach (GameUser user in users)
                {
                    if (IdsAlreadyInList.Contains(user.UserId))
                    {
                        // This user was already returned by another means, e.g. username and now display name, so just return
                        return;
                    }

                    Debug.Log("User: " + user.UserName);
                    IdsAlreadyInList.Add(user.UserId);

                    NetworkManager.CallOnMainThread(() =>
                    {
                        ButtonAddFriend newButton = Instantiate(buttonAddFriendPrefab);
                        newButton.iconOfFriend = null; // TODO get Facebook pictures
                        newButton.button.GetComponentInChildren<Text>().text = "Add friend: " + user.UserName;
                        newButton.usernameOfFriend = user.UserName;
                        newButton.userIdOfFriend = user.UserId;
                        newButton.button.onClick.AddListener(newButton.addFriend);
                        newButton.transform.SetParent(contentPanel);
                        newButton.transform.localScale = new Vector3(1, 1, 1);
                    });
                }
            }
        });
    }
}