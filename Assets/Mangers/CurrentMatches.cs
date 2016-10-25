using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Networking;
using UnityEngine.SceneManagement;
using Assets.Mangers;
using System;

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
        _networkManager.GetMatchesAsync(matches =>
        {
            foreach (LevelDefinition match in matches)
            {
                // Need to clone match because the async call overwrites the reference it seems. Need to research.
                LevelDefinition clonedMatch = match.Clone();

                NetworkManager.CallOnMainThread(() => AddMatchButton(clonedMatch));
            }
        });
    }

    private void AddMatchButton(LevelDefinition match)
    {
        string friendName;
        string friendId;
        if (_networkManager.CurrentUser.UserId == match.PlayerLeftId)
        {
            friendName = match.PlayerRightName;
            friendId = match.PlayerRightId;
        }
        else if(_networkManager.CurrentUser.UserId == match.PlayerLeftId)
        {
            friendName = match.PlayerLeftName;
            friendId = match.PlayerLeftId;
        }
        else
        {
            return;
            //throw new Exception("Loading match where current user isn't a player!");
        }

        ButtonCurrentMatch newButton = Instantiate(buttonCurrentMatchPrefab);
        newButton.iconOfFriend = null; // TODO: Add icon from Facebook or something
        newButton.button.GetComponentInChildren<Text>().text = "Match in Progress: " + friendName;
        newButton.usernameOfFriend = friendName;
        newButton.userIdOfFriend = friendName;
        //newButton.matchId = _networkManager.getgr;
        newButton.levelDefinition = match;
        newButton.button.onClick.AddListener(newButton.LoadMatch);
        newButton.transform.SetParent(matchPanelContent);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        newButton.playerLeftHealth = match.PlayerLeftHealth;
        newButton.playerRightHealth = match.PlayerRightHealth;
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
