using UnityEngine;
using UnityEngine.UI;
using Parse;
using System;
using UnityEngine.SceneManagement;

public class CreateAccount : MonoBehaviour
{
    public Button SignInButton;
    public Button SignUpButton;
    public InputField UserNameField;
    public InputField PasswordField;
    public Text ErrorText;

    // Use this for initialization
    void Start ()
    {
        SignInButton.onClick.AddListener(TestSignInButtonBehavior);
        SignUpButton.onClick.AddListener(TestSignUpButtonBehavior);

        ErrorText.enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    public void TestSignUpButtonBehavior()
    {
        ParseUser user = new ParseUser()
        {
            Username = UserNameField.text,
            Password = PasswordField.text
            //,Email = null
        };

        // other fields can be set just like with ParseObject
        //user["phone"] = "650-555-0000";
       
        user.SignUpAsync().ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Debug.Log("Canceled Sign Up");
            }
            if (t.IsFaulted)
            {
                Debug.Log("Sign up faulted");

                foreach (var ex in t.Exception.InnerExceptions)
                {
                    ParseException parseException = (ParseException)ex;
                    Debug.Log("Error message " + parseException.Message);
                    Debug.Log("Error code: " + parseException.Code);
                    NetworkManager.Call(ShowErrorText, ex.Message);
                }
            }
            else
            {
                // Login was successful.
                Debug.Log("Sign up success");
                NetworkManager.Call(HideErrorText);
                NetworkManager.Call(LoadCreateMatch);
            }
        });
    }

    public void TestSignInButtonBehavior()
    {
        // TODO save actual username as lowercase so that we don't have dups, but also save display name with cases.
        ParseUser.LogInAsync(UserNameField.text, PasswordField.text).ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Debug.Log("Canceled Sign In");
            }
            if (t.IsFaulted)
            {
                Debug.Log("Sign in faulted");
                foreach (var ex in t.Exception.InnerExceptions)
                {
                    ParseException parseException = (ParseException)ex;
                    Debug.Log("Error message " + parseException.Message);
                    Debug.Log("Error code: " + parseException.Code);
                    NetworkManager.Call(ShowErrorText, parseException.Message);
                }
            }
            else
            {
                // Login was successful.
                Debug.Log("Sign in success");
                NetworkManager.Call(HideErrorText);
                NetworkManager.Call(LoadCreateMatch);
            }
        });
    }

    private void LoadFriendSearch()
    {
        SceneManager.LoadScene("FriendSearch");
    }

    private void LoadCreateMatch()
    {
        SceneManager.LoadScene("CreateMatch");
    }

    public void ShowErrorText(object error)
    {
        Debug.Log("SHowErrorText");
        ErrorText.enabled = true;
        ErrorText.text = (String)error;
    }
    private void HideErrorText()
    {
        if (ErrorText != null && ErrorText.enabled)
        {
            ErrorText.enabled = false;
        }
    }
}
