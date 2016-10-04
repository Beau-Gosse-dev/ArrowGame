using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Networking;
using UnityEngine.SceneManagement;

public class CurrentMatches : MonoBehaviour {
    
    public Text myUsername;
    public RectTransform matchPanelContent;
    public ButtonCurrentMatch buttonCurrentMatchPrefab;
    
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
        LoadMatchButtonsAsync();
    }

    public void LoadMatchButtonsAsync()
    {
        //
        // Populate match buttons where current player is left player or the right player
        //
        _networkManager.GetMatchesAsync().ContinueWith(t =>
        {
            foreach (var match in t.Result)
            {
                // We can only instantiate objects (in this case buttons) on the main thread, so we have a crazy work around to add functions to the main thread.
                // TODO: Find a better way to instantiate on other threads
                NetworkManager.Call(() => AddMatchButton(match));
            }
        });
    }

    private void AddMatchButton(Match match)
    {
        ButtonCurrentMatch newButton = Instantiate(buttonCurrentMatchPrefab);
        newButton.iconOfFriend = null; // TODO: Add icon from Facebook or something
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

    public void loadFriendSeach()
    {
        SceneManager.LoadScene("FriendSearch");
    }

    public void loadCreateMatch()
    {
        SceneManager.LoadScene("CreateMatch");
    }
}
