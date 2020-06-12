using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class GameplayHandler : MonoBehaviourPun
{
    string playerSequence;
    bool isGameOver;

    public GamePlayUIManager _GamePlayUIManager;
    public PlayerHandler _PlayerHandler;
    public TurnManager _TurnManager;
    public OnlinePlayerHandler _OnlinePlayerHandler;
    public TokenManager _TokenManager;
    public Action<Token.TokenPlayer> OnWinnerFound;


    private void Awake()
    {
       
    }
    private void OnEnable()
    {
        _TurnManager = FindObjectOfType<TurnManager>();
        _PlayerHandler = FindObjectOfType<PlayerHandler>();
        _GamePlayUIManager = FindObjectOfType<GamePlayUIManager>();
        _TokenManager = FindObjectOfType<TokenManager>();
        _PlayerHandler.OnWinnerFound += WinnerFound;
       
        if (_OnlinePlayerHandler != null)
            _OnlinePlayerHandler.onPlayerLeftRoom+= MultiplayerHandler_OnPlayerLeftRoom;
    }

    void MultiplayerHandler_OnPlayerLeftRoom(Photon.Realtime.Player obj)
    {
        if (!isGameOver)
            WinnerFound(_TurnManager.GetCurrentTokenPlayer());
    }
    void WinnerFound(Token.TokenPlayer player)
    {
        GameOver(player);
        if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
            photonView.RPC("RPC_GameOver", RpcTarget.Others, (int)_TurnManager.GetCurrentTokenType());
    }


    void GameOver(Token.TokenPlayer player)
    {
        Debug.Log("Gamever>>>>>>>>>>>>>>>>>>>>>>");
         isGameOver = true;
        _GamePlayUIManager.SetEnable(true);
        _GamePlayUIManager.ShowGameOver();
        //  gameOverScreen.UpdateWinnerPlayerText(winningPlayer);
    }
    void GameOver_Other(GameObject player)
    {
        Debug.Log("Gamever>>>>>>>>>>>>>>>>>>>>>>");
        isGameOver = true;
        _GamePlayUIManager.SetEnable(true);
        _GamePlayUIManager.ShowGameOver();
        //  gameOverScreen.UpdateWinnerPlayerText(winningPlayer);
    }
    [PunRPC]
    public void RPC_GameOver(int index)
    {
        Token.TokenType playerType = (Token.TokenType)index;
        GameOver_Other(_TokenManager.ActivePlayers[index].gameObject);
    }

    private void Start()
    {
        if (GameMaster.instance != null)
        {
            GameMaster.instance._GameplayHandler = this;

            if (GameMaster.instance._GamePlayType == GamePlayType.OFFLINE)
            {//
              //  playerSequence = GameMaster.instance.GetPlayersSequence(2);
             //   _PlayerManager.InitializeAllPlayers(playerSequence);
             //   _PlayerManager.StartPlayerTurn();
            }
            else
            {
                Debug.Log("Play Online");
                if(_PlayerHandler==null)
                    Debug.Log("_PlayerHandler Null ");
                Invoke("PlayerInit", 1f);
            }
        }
    }

    public void PlayerInit()
    {
        playerSequence = GameMaster.instance.GetPlayersSequence(NetworkHandler.ExpectedMaxPlayers);
        _PlayerHandler.InitializeAllPlayers(playerSequence);
    }


    private void OnDisable()
    {
        _PlayerHandler.OnWinnerFound -= WinnerFound;

        if(_OnlinePlayerHandler != null)
            _OnlinePlayerHandler.onPlayerLeftRoom -= MultiplayerHandler_OnPlayerLeftRoom;
    }
}