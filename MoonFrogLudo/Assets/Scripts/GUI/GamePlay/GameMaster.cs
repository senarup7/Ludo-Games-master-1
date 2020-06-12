using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;
    public GamePlayType _GamePlayType;
  //  public UIManager _UIManager;
 //   public SceneController _SceneController;
    public GameplayHandler _GameplayHandler;
    [HideInInspector]
    public  int OnlineMaxPlayer = 2;
    public Token.TokenType[] SelectedTokens {
		get;
		set;
	}

	public Token.TokenPlayer[] SelectedTokenPlayers {
		get;
		set;
	}

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (instance);

        _GamePlayType = GamePlayType.ONLINE;

    }


    void Start ()
	{

        if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
        {
            SelectedTokens = new Token.TokenType[]{
            Token.TokenType.Blue, Token.TokenType.Red
            };
        }
        else
        {
            SelectedTokens = new Token.TokenType[]{
            Token.TokenType.Blue, Token.TokenType.Red, Token.TokenType.Green, Token.TokenType.Yellow
            };
        }
	

        if (_GamePlayType == GamePlayType.OFFLINE)
        {
            SelectedTokenPlayers = new Token.TokenPlayer[] {
            Token.TokenPlayer.Opponent, Token.TokenPlayer.Opponent, Token.TokenPlayer.Opponent, Token.TokenPlayer.Opponent
            };
        }
        if (_GamePlayType == GamePlayType.ONLINE)
        {
            SelectedTokenPlayers = new Token.TokenPlayer[] {
            Token.TokenPlayer.Player, Token.TokenPlayer.Player
            };
        }

    }

    public string GetPlayersSequence(int numberOfPlayers)
    {
        string _GamePlayers = "";
        int maxPlayerCount = numberOfPlayers;
        switch (maxPlayerCount)
        {
            case 2:
                _GamePlayers = "0:1";
                break;
            case 3:
                _GamePlayers = "0:1:2";
                break;
            case 4:
                _GamePlayers = "0:1:3";
                break;
        }
        return _GamePlayers;
    }
}
public enum GamePlayType
{
    ONLINE,
    OFFLINE
}