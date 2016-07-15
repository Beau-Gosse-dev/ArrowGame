using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Networking;

public class CurrentMatches : MonoBehaviour {

    public Button createMatchButton;
    public Text myUsername;
    public RectTransform matchPanelContent;
    public ButtonCurrentMatch buttonCurrentMatchPrefab;
    public Button AddFriendButton;
    
    private List<ButtonCurrentMatch> matchButtonList = new List<ButtonCurrentMatch>();
    private NetworkManager _networkManager;

    void Start ()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        myUsername.text = _networkManager.CurrentUser.UserName;
        LoadMatchButtonsAsync();
    }

    private void AddMatchButton(Match match)
    {
        ButtonCurrentMatch newButton = Instantiate(buttonCurrentMatchPrefab);
        newButton.iconOfFriend = null; // TODO: Add icon from facebook or something
        newButton.button.GetComponentInChildren<Text>().text = "Match in Progress: " + match.friendName;
        newButton.usernameOfFriend = match.friendName;
        newButton.userIdOfFriend = match.friendId;
        newButton.matchId = match.matchId;
        newButton.button.onClick.AddListener(newButton.LoadMatch);
        newButton.transform.SetParent(matchPanelContent);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        newButton.playerLeftHealth = match.leftHealth;
        newButton.playerRightHealth = match.rightHealth;
        loadHealthSliders(newButton);
    }

    private void loadHealthSliders(ButtonCurrentMatch newButton)
    {
        IList<Slider> sliders = newButton.button.GetComponentsInChildren<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.name == "LeftHealthSlider")
            {
                slider.value = newButton.playerLeftHealth;
            }
            else
            {
                slider.value = newButton.playerRightHealth;
            }
            // If the slider is at 0, remove the green "fill" image
            if (slider.value <= 0)
            {
                foreach (Image img in slider.GetComponentsInChildren<Image>())
                {
                    if (img.name == "Fill")
                    {
                        img.enabled = false;
                    }
                }
            }
        }
    }

    public void LoadMatchButtonsAsync()
    {
        //
        // Populate match buttons where current player is left player or the right player
        //
        var matches = _networkManager.GetMatchesAsync().ContinueWith(t =>
        {
            foreach (var match in t.Result)
            {
                AddMatchButton(match);
            }
        });
    }
}
