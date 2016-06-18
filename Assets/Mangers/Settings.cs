using UnityEngine;
using System.Collections;
using Parse;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public Button LogoutButton;
    public Button BackButton;
    public Image PendingImage;
    public Text ButtonText;
    public void Start()
    {
        BackButton.onClick.AddListener(() => { Application.LoadLevel("StartMenu"); });
        if (ParseUser.CurrentUser != null)
        {
            LogoutButton.enabled = true;
            ButtonText.text = "Logout";
        }
        else
        {
            LogoutButton.enabled = false;
            ButtonText.text = "Not Signed In";
        }
    }

    public void onLogoutClicked()
    {
        LogoutButton.enabled = false;
        PendingImage.enabled = true;
        if (ParseUser.CurrentUser == null)
        {
            Application.LoadLevel("CreateAccount");
        }
        else
        {
            ParseUser.LogOutAsync().ContinueWith(T =>
            {
                NetworkManager.Call(() =>
                {
                    PendingImage.enabled = false;
                    LogoutButton.enabled = false;
                    ButtonText.text = "Not Signed In";
                // TODO maybe show progress to make sure a user isn't doing anything when they think they are logged out
                // Handle errors if the logout didn't work.
            });
            });
        }
    }
}
