using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CreateMatch : MonoBehaviour {

    public Button createMatchButton;
    public Text myUsername;
    public RectTransform userPanelContent;
    public ButtonCreateMatch buttonCreateMatchPrefab;

    public Button AddFriendButton;
    
    private List<ButtonCreateMatchContent> userButtonContentList = new List<ButtonCreateMatchContent>();

    // Use this for initialization
    void Start ()
    {
        myUsername.text = ParseUser.CurrentUser.Username;
        FindFriends();
    }
    

    private void FindFriends()
    {
        //
        // Populate friend buttons for creating a new match
        // 
        ParseUser currentUser = ParseUser.CurrentUser;
        try
        {
            IList<string> friends = currentUser.Get<IList<string>>("friends");
            foreach (string friend in friends)
            {
                ParseUser.Query.GetAsync((string)friend).ContinueWith(t =>
                {
                    string username = t.Result.Username;
                    string userid = t.Result.ObjectId;
                    NetworkManager.Call(() =>
                    {
                        ButtonCreateMatch newButton = Instantiate(buttonCreateMatchPrefab);

                        newButton.iconOfFriend = null; //todo add pic
                        newButton.button.GetComponentInChildren<Text>().text = "Create New with: " + username;
                        newButton.usernameOfFriend = username;
                        newButton.userIdOfFriend = userid;
                        newButton.button.onClick.AddListener(newButton.createMatch);
                        newButton.transform.SetParent(userPanelContent);
                        newButton.transform.localScale = new Vector3(1, 1, 1);
                    });
                });
            }
        }
        catch (KeyNotFoundException)
        {
            // This user hasn't added friends yet. Do nothing. 
        }
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
