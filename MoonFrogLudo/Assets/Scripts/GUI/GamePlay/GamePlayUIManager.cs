using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayUIManager : MonoBehaviour {

    
    public GameObject GameOverPanel;
    [SerializeField] private QuestionDialog questionDialog ;
    [SerializeField] private ShowDialogGameOver showDialogGameOver;
    private void Awake()
    {

        GameOverPanel.SetActive(false);
    }


    public void SetEnable(bool val)
    {
        GameOverPanel.SetActive(val);
    }
    void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			questionDialog.ShowDialog ("Are you sure want to end this game?", ExitFromRoom, null);
		}
	}

	void GoToMainMenu () 
	{
		SceneManager.LoadScene ("MainMenu");
	}

    public void BackButtonAction()
    {
        ExitFromRoom();
    }

    public void ExitFromRoom()
    {
        if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
        {
            GameMaster.instance._GameplayHandler._OnlinePlayerHandler.Disconnect();
        }

        GoToMainMenu();
    }
    public void ShowGameOver()
    {
        showDialogGameOver.ShowGameOverDialog("You Won The Match", GoToMainMenu);
    }
    public void ShowGameOverScreenText()
    {
        showDialogGameOver.ShowGameOverDialog("Congratulation..............", GoToMainMenu);
    }
}
