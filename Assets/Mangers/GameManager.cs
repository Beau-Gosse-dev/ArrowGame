using Assets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameManager
{
    //#region private members
    //private GameState GameState;
    //private bool IdentityRegistered;
    //private string MostRecentMatchIdentifier;
    //#endregion

    ////#region handlers
    ////public delegate void StatesAvailableHandler(List<GameState.MatchState> MatchStates);
    ////#endregion

    //#region Singleton
    //// Manager is a singleton throughout the game lifecycle.
    //private static GameManager _instance = null;
    //public static GameManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new GameManager();
    //        }
    //        return _instance;
    //    }
    //}

    //private GameManager()
    //{
    //    IdentityRegistered = false;
    //    MostRecentMatchIdentifier = "";
    //}
    //# endregion
    ////public void LogInToFacebook()
    ////{
    ////    NetworkManager.Instance.LogInToFacebookAsync();
    ////}


    public static void loadFriendSeach()
    {
        Application.LoadLevel("FriendSearch");
    }
    
}
