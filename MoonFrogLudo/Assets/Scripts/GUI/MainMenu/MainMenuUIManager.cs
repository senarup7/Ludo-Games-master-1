using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
public class MainMenuUIManager : MonoBehaviour {


    public static MainMenuUIManager Instance { get; private set; }

   
    [SerializeField] private GameObject gamePlayPreference;
    [SerializeField] private GameObject ConnectPVP;
    [SerializeField] private QuestionDialog quitDialog;

	

	private int playerCount = 2;
	private Token.TokenType selectedToken = Token.TokenType.Blue;

  
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
            Instance = this;

       // PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start ()
	{
		
        if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
        {
            playerCount = 2;
        }
        else
        {
          
        }
		
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (gamePlayPreference.activeSelf) {
				gamePlayPreference.SetActive (false);
			} else {
				quitDialog.ShowDialog ("Are you sure want to quit?", () => Application.Quit (), null);
			}
		}
	}
   
    public void OnVSComputer ()
	{
		gamePlayPreference.SetActive (true);
	}
    public void OnPVP()
    {
        ConnectPVP.SetActive(true);
    }

    public void OnPlay ()
	{

     
		Token.TokenPlayer[] players = new Token.TokenPlayer[playerCount];
		Token.TokenType[] types = new Token.TokenType[playerCount];

		for (int i = 0; i < playerCount; i++) {

            if(GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
            {
                players[i] = Token.TokenPlayer.Player;
            }
            else
            {
                players[i] = Token.TokenPlayer.Opponent;
            }
			
			types [i] = (Token.TokenType)i;

			if (types [i] == selectedToken) {
				players [i] = Token.TokenPlayer.Player;
			}
		}

		if ((int)selectedToken >= playerCount) {
			players [playerCount - 1] = Token.TokenPlayer.Player;
			types [playerCount - 1] = selectedToken;
		}
        Debug.Log("Player Count : " + playerCount);
		GameMaster gm = GameMaster.instance;
		gm.SelectedTokens = types;
		gm.SelectedTokenPlayers = players;
      
        PhotonNetwork.LoadLevel("GamePlay");
      //  SceneManager.LoadScene ("GamePlay");
	}

}
