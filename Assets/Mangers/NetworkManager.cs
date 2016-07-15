using UnityEngine;
using Facebook.Unity;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System;
using Parse;
using UnityEngine.SceneManagement;

class NetworkManager : MonoBehaviour
{
    #region One-time Creation
    public static NetworkManager Instance { get; private set; }
    void Awake()
    {
        // For running on the main thread
        calls = new List<CallInfo>();
        functions = new List<Func>();
        StartCoroutine(Executer());

        Instance = this;
        // This NetworkManager object will persist until the game is closed
        DontDestroyOnLoad(gameObject);

        // Now that the network manager is ready, head to the menu
        SceneManager.LoadScene("StartMenu");
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    public bool IsUserLoggedIn()
    {
        return ParseUser.CurrentUser != null;
    }

    #region Parse Stuff
    class CallInfo
    {
        public Function func;
        public object parameter;
        public CallInfo(Function Func, object Parameter)
        {
            func = Func;
            parameter = Parameter;
        }
        public void Execute()
        {
            func(parameter);
        }
    }

    public delegate void Function(object parameter);
    public delegate void Func();

    static List<CallInfo> calls = new List<CallInfo>();
    static List<Func> functions = new List<Func>();


    static System.Object callsLock = new System.Object();
    static System.Object functionsLock = new System.Object();

    public static void Call(Function Func, object Parameter)
    {
        lock (callsLock)
        {
            calls.Add(new CallInfo(Func, Parameter));
        }
    }
    public static void Call(Func func)
    {
        lock (functionsLock)
        {
            functions.Add(func);
        }
    }

    IEnumerator Executer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            while (calls.Count > 0)
            {
                calls[0].Execute();
                lock (callsLock)
                {
                    calls.RemoveAt(0);
                }
            }

            lock (functionsLock)
            {
                while (functions.Count > 0)
                {
                    if (functions != null)
                    {
                        if (functions[0] != null)
                        {
                            functions[0]();
                            functions.RemoveAt(0);
                        }
                    }
                }
            }
        }
    }
    #endregion
}
