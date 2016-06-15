using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Parse;

public class StartMenu : MonoBehaviour
{
    public Button playHumanButton;
    public Button playComputerButton;
    public Button playFriendButton;
    public Button settingsButton;

	// Use this for initialization
	void Start ()
    {
        //playHumanButton = GetComponent<Button>();
        //playComputerButton = GetComponent<Button>();
        //playFriendButton = GetComponent<Button>();
        //settingsButton = GetComponent<Button>();
    } // End Start

    public void onHumanClicked()
    {
        LevelDefinition.gameType = GameType.Local;
        Application.LoadLevel("Human");
    } // End onHumanClicked    
    public void onComputerClicked(){
        LevelDefinition.LevelDefinitionSetDefault();
        LevelDefinition.gameType = GameType.Computer;
        LevelDefinition.WallPosition = -1;
        Application.LoadLevel("Computer");
    } // End onHumanClicked

    public void onSettingsClicked()
    {
        Application.LoadLevel("Settings");
    } // End onSettingsClicked

    public void onFriendClicked()
    {
        if(ParseUser.CurrentUser != null)
        {
            Application.LoadLevel("CurrentMatches");
        }
        else
        {
            Application.LoadLevel("CreateAccount");
        }
    } // End onFriendClicked
}
