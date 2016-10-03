using UnityEngine;
using UnityEngine.UI;
using Parse;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FriendSearch : MonoBehaviour {
    
    public Button SearchButton;
    public Button GoToCreateMatchButton;
    public InputField UserNameField;
    public Text myUsername;
    public Transform contentPanel;
    public ButtonAddFriend buttonAddFriendPrefab;
    
    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
    }

    // Use this for initialization
    void Start ()
    {
        myUsername.text = ParseUser.CurrentUser.Username;
    }

    public void GoToCreateMatch()
    {
        SceneManager.LoadScene("CreateMatch");
    }

    public void SearchUser ()
    {
        string searchString = UserNameField.text;

        ParseUser.Query
            .WhereStartsWith("username", searchString)
            .WhereNotEqualTo("objectId", ParseUser.CurrentUser.ObjectId) // Don't search for yourself
            .FindAsync().ContinueWith(t =>
            {
                IEnumerable<ParseUser> users = t.Result;
                foreach (ParseUser user in users)
                {
                    ParseUser userForLambda = user;
                    Debug.Log("User" + user.Username);
                    NetworkManager.Call(() =>
                    {
                        ButtonAddFriend newButton = Instantiate(buttonAddFriendPrefab);
                        newButton.iconOfFriend = null; // Todo get facebook pictures
                        newButton.button.GetComponentInChildren<Text>().text = userForLambda.Username;
                        newButton.usernameOfFriend = userForLambda.Username;
                        newButton.userIdOfFriend = userForLambda.ObjectId;
                        newButton.button.onClick.AddListener(newButton.addFriend);
                        newButton.transform.SetParent(contentPanel);
                        newButton.transform.localScale = new Vector3(1, 1, 1);
                    });
                }
            });
    }

    public static void addFriend(string userId)
    {
        ParseUser.CurrentUser.AddUniqueToList("friends", userId);
        ParseUser.CurrentUser.SaveAsync();
    }
}