using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateMatch : MonoBehaviour {

    public Button createMatchButton;
    public Text myUsername;
    public RectTransform userPanelContent;
    public ButtonCreateMatch buttonCreateMatchPrefab;

    public Button AddFriendButton;

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
        FindFriends();
    }
    

    private void FindFriends()
    {
        _networkManager.loadFriends((string userName, string userId) =>
        {
            ButtonCreateMatch newButton = Instantiate(buttonCreateMatchPrefab);

            newButton.iconOfFriend = null; // TODO add picture
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
