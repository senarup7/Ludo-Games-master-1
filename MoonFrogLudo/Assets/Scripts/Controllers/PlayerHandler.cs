using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.UI;
public class PlayerHandler : MonoBehaviourPun
{

    public Action<Token.TokenPlayer> OnWinnerFound;
    public Text PlayerText;

    TurnManager _TurnManager;
   // private int activePlayerIndex = -1;
    public GameObject currentPVPPlayer = null;
    public GameObject currentActivePlayer = null;

    public String PlayerType;

    public TokenManager tokenManager;

    public DiceManager diceManager;
    private void Awake()
    {
        diceManager = FindObjectOfType<DiceManager>();

        _TurnManager = FindObjectOfType<TurnManager>();

        tokenManager = FindObjectOfType<TokenManager>();
    }
    public void InitializeAllPlayers(string activePlayerSequence)
    {
        if (GameMaster.instance._GamePlayType == GamePlayType.OFFLINE)
        {
            PhotonNetwork.OfflineMode = true;
            InitPlayers(activePlayerSequence);
        }
        else
        {
            InitPlayers(activePlayerSequence);
          
        }
    }


    void InitPlayers(string activePlayerSequence)
    {

    //    activePlayerIndex = -1;
        string[] sequenceArray = activePlayerSequence.Split(':');
        for (int i = 0; i < tokenManager.ActivePlayers.Count; i++)
        {
            if (i == int.Parse(sequenceArray[i]))
            {
                tokenManager.ActivePlayers[i].gameObject.SetActive(true);
            }
           
        }
       
        Invoke("SetCurrentPVPPlayer",0.1f);


        
    }

    public void SetCurrentPVPPlayer()
    {


      //  Token.TokenPlayer tokenPlayer = _TurnManager.CurrentPlayer();
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                if (PhotonNetwork.CurrentRoom.Players.Count == NetworkHandler.ExpectedMaxPlayers)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {

                        if (PhotonNetwork.LocalPlayer == player)
                        {
                             

                              currentPVPPlayer = tokenManager.ActivePlayers[player.ActorNumber-1];

                              // currentPVPPlayer.dashboard.IndicatePVPCurrentPlayer();
                              Debug.Log("currentPVPPlayer " + currentPVPPlayer.name);
                              PlayerText.text = currentPVPPlayer.name;
                           
                            

                        }
                    }
                    
                }
            }
        }

      //  if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
           // photonView.RPC("StartPlayerTurn", RpcTarget.Others);
    }
  
    public bool IsMyTurnForPVP()
    {
   
        return currentPVPPlayer == currentActivePlayer;
    }

    [PunRPC]
    void OnWinnerPlayerFound(Token.TokenPlayer player)
    {
        OnWinnerFound?.Invoke(player);
    }

    private void OnDisable()
    {

        foreach (var player in tokenManager.ActivePlayers)
        {
          //  player.OnAllPiecesComplete -= OnWinnerPlayerFound;
        }
    }

}
