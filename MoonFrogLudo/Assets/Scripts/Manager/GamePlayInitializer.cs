using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayInitializer : MonoBehaviour {
    TurnManager turnManager;
    void Start ()
	{
		GameMaster gm = GameMaster.instance;
        MainMenuUIManager MenuManager = GetComponent<MainMenuUIManager>();
        DiceManager diceManager = GetComponent<DiceManager> ();
		TokenManager tokenManager = GetComponent<TokenManager> ();
		 turnManager = GetComponent<TurnManager> ();
		WaypointManager waypointManager = GetComponent<WaypointManager> ();
		HomeBaseManager hbManager = GetComponent<HomeBaseManager> ();
		OpponentController opponentCtrl = GetComponent<OpponentController> ();
		BoardHighlighter highlighter = GameObject.Find ("Board").GetComponent<BoardHighlighter> ();
        PlayerHandler playerHandler = GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>();
        diceManager.Init (gm.SelectedTokens);
		opponentCtrl.DiceManager = diceManager;
        if(gm._GamePlayType==GamePlayType.ONLINE)
		    tokenManager.Init (waypointManager, hbManager, gm.SelectedTokens, gm.SelectedTokenPlayers);
       // else
         //   tokenManager.Init(waypointManager, hbManager, gm.SelectedTokens, gm.SelectedTokenPlayers);


        turnManager.Init (tokenManager, diceManager, opponentCtrl, highlighter);

		turnManager.StartFirstTurn ();
	}

}
