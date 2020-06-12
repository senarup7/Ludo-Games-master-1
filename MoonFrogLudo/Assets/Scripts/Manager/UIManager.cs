
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{

    public static UIManager Inst;
    internal RectTransform ParentRect;
    internal RectTransform ParentRectLeft;
    internal RectTransform ParentRectRight;


    internal string profileId = string.Empty;



    public List<UIScreen> previousScenes = new List<UIScreen>();
    //  public List<GameObject> Screens = new List<GameObject>();
    UnityAction ActionCancel;
    UnityAction ActionOk;


    void Awake()
    {
        Inst = this;
    }
    // Use this for initialization
    void Start()
    {
        ActionOk = new UnityAction(QuitFromApp);
        ActionCancel = new UnityAction(CancelQuit);

        previousScenes.Add(UIScreen.MainMenu);


    }

    void QuitFromApp()
    {
        Application.Quit();
        print("Quit");
    }


    void CancelQuit()
    {
        print("Cancel Login");
    }

    /// <summary>
    /// Raises the level was loaded event.
    /// </summary>
    /// <param name="level">Level.</param>
    internal void OnLevelWasLoaded()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Parent");
        if (go != null)
        {
            ParentRect = go.GetComponent<RectTransform>();
        }
        else
        {

        }
    }
    void Update()
    {




        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ActionOk = new UnityAction(QuitFromApp);
                ActionCancel = new UnityAction(CancelQuit);

                UIScreen screen = UIManager.Inst.previousScenes[UIManager.Inst.previousScenes.Count - 1];

                switch (screen)
                {
                    case UIScreen.MainMenu:
                        AlertPopupManager.Inst.Show("Want to quit?", "Press OK to Confirm", new string[] { "CANCEL", "OK" }, new UnityAction[] { ActionCancel, ActionOk });
                        Application.Quit();
                        break;
                    case UIScreen.OfflineLobby:
                        OnBackFromTable();
                        screen = UIScreen.None;
                        break;
                    case UIScreen.OnlineLobby:
                  //      SignupScreenManager.Inst.OnBackButton();
                        screen = UIScreen.None;
                        break;
                    case UIScreen.PlayOffLine:
                  //      ForgotPasswordManger.Inst.BackBtnClick();
                        screen = UIScreen.None;
                        break;
                    case UIScreen.PlayOnline:
                        //      ForgotPasswordManger.Inst.BackBtnClick();
                        screen = UIScreen.None;
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Loads the screen.
    /// </summary>
    /// <param name="scr">Scr.</param>
    internal void LoadScreen(UIScreen scr)
    {
        switch (scr)
        {
            case UIScreen.MainMenu:
                SceneManager.LoadScene(SceneNameConstants.MAINMENU);
                break;
            case UIScreen.OfflineLobby:
                SceneManager.LoadScene(SceneNameConstants.OFFLINELOBBY);
                break;
            case UIScreen.OnlineLobby:
                SceneManager.LoadScene(SceneNameConstants.ONLINELOBBY);
                break;
            case UIScreen.PlayOnline:
                SceneManager.LoadScene(SceneNameConstants.ONLINEPLY);
                break;
            case UIScreen.PlayOffLine:
                SceneManager.LoadScene(SceneNameConstants.OFFLINEPLAY);
                break;
        }
    }

    internal void MoveOut()
    {
        ParentRect.GetComponent<Animator>().SetBool("isMove", true);
    }

    internal void MoveIN()
    {
        ParentRect.GetComponent<Animator>().SetBool("isMove", false);
    }
    /// <summary>
    /// Raises the insta play event.
    /// </summary>
    public void OnBackFromTable()
    {
        RemoveSceneData();

    }
    public void OnBackToBuyChips()
    {
        RemoveSceneData();
  
    }
    public void RemoveSceneData()
    {

        //   for (int i = 0; i < 4;i++){
        UIManager.Inst.previousScenes.RemoveAt(UIManager.Inst.previousScenes.Count - 1);
        //  }
    }
}

public enum UIScreen
{
    None,
    MainMenu,
    OfflineLobby,
    OnlineLobby,
    PlayOnline,
    PlayOffLine


}
public class SceneNameConstants
{
    public const string MAINMENU = "MainMenu";
    public const string OFFLINELOBBY = "LobbyOfLine";
    public const string ONLINELOBBY = "LobbyOnLine";
    public const string ONLINEPLY = "GameOnline";
    public const string OFFLINEPLAY = "GameOffLine";

}