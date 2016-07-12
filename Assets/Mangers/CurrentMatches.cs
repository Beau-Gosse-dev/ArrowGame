using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CurrentMatches : MonoBehaviour {

    public Button createMatchButton;
    public Text myUsername;
    public RectTransform matchPanelContent;
    public ButtonCurrentMatch buttonCurrentMatchPrefab;
    public Button AddFriendButton;
    
    private List<ButtonCurrentMatch> matchButtonList = new List<ButtonCurrentMatch>();

    // Use this for initialization
    void Start ()
    {
        myUsername.text = ParseUser.CurrentUser.Username;
        FindMatches();
    }

    public void FindMatches()
    {
        //
        // Populate matche buttons where current player is left player or the right player
        // 
        ParseQuery<ParseObject> queryLeftPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerLeftId", ParseUser.CurrentUser.ObjectId);
        ParseQuery<ParseObject> queryRightPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerRightId", ParseUser.CurrentUser.ObjectId);
        ParseQuery<ParseObject> queryBothSides = queryLeftPlayer.Or(queryRightPlayer);
        queryBothSides.FindAsync().ContinueWith(t =>
        {
            IEnumerable<ParseObject> results = t.Result;
            foreach (ParseObject matchObject in results)
            {
                ParseObject matchParseObject = matchObject;

                NetworkManager.Call(() =>
                {
                    ButtonCurrentMatch newButton = Instantiate(buttonCurrentMatchPrefab);
                    // If the current user is the match's left player, show the right player on the button (the opponent)
                    string friendId;
                    string friendName;
                    if (matchParseObject.Get<string>("playerLeftId") == ParseUser.CurrentUser.ObjectId)
                    {
                        friendId = matchParseObject.Get<string>("playerRightId");
                        friendName = matchParseObject.Get<string>("playerRightName");
                    }
                    else
                    {
                        friendId = matchParseObject.Get<string>("playerLeftId");
                        friendName = matchParseObject.Get<string>("playerLeftName");
                    }

                    newButton.iconOfFriend = null; //todo
                    newButton.button.GetComponentInChildren<Text>().text = "Match in Progress: " + friendName;
                    newButton.usernameOfFriend = friendName;
                    newButton.userIdOfFriend = friendId;
                    newButton.matchId = matchParseObject.ObjectId;
                    newButton.button.onClick.AddListener(newButton.LoadMatch);
                    newButton.transform.SetParent(matchPanelContent);
                    newButton.transform.localScale = new Vector3(1, 1, 1);
                    newButton.playerLeftHealth = matchParseObject.Get<float>("playerLeftHealth");
                    newButton.playerRightHealth = matchParseObject.Get<float>("playerRightHealth");
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
                });
            }
        });
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
