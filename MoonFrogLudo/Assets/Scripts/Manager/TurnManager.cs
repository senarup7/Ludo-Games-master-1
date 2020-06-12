using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TurnManager : MonoBehaviourPun
{
    public delegate void OnNextTurn(Token.TokenType tokenType);
    public delegate void OnSixThreeTimes(Token.TokenType tokenType);

    public event OnNextTurn onNextTurn;
    public event OnSixThreeTimes onSixThreeTimes;

    //	private Dictionary<Token.TokenType, GameObject[]> tokens;
    //	public Dictionary<Token.TokenType, GameObject[]> Tokens {
    //		get { return tokens; }
    //		set { tokens = value; }
    //	}
    //

    private PlayerHandler playerHandler;
    private TokenManager tokenManager;
    private Dice dice;
    private DiceManager diceManager;
    private OpponentController opponentCtrl;
    private BoardHighlighter highlighter;
    private Token.TokenType[] selectedTypes;
    private int currentToken = 0;
    private int lastDiceNum = 0;
    private int diceSixCount = 0;

    public void Init(TokenManager tokenManager, DiceManager diceManager,
        OpponentController oppCtrl, BoardHighlighter highlighter)
    {
        this.diceManager = diceManager;
        this.opponentCtrl = oppCtrl;
        this.highlighter = highlighter;
        this.tokenManager = tokenManager;

        opponentCtrl.onTokenSelected += TokenSelected;
        opponentCtrl.TokenManager = tokenManager;
        diceManager.onDiceRolled += DiceRolled;
        dice = diceManager.GetCurrentActiveDice();

        tokenManager.onTokenAnimationsDone += TokenAnimationsEnd;

        selectedTypes = new Token.TokenType[tokenManager.Tokens.Keys.Count];
        tokenManager.Tokens.Keys.CopyTo(selectedTypes, 0);

        foreach (KeyValuePair<Token.TokenType, Token[]> entry in tokenManager.Tokens)
        {
            for (int i = 0; i < entry.Value.Length; i++)
            {
                Token t = entry.Value[i].GetComponent<Token>();
                t.onTokenSelected += TokenSelected;
            }
        }

        playerHandler = FindObjectOfType<PlayerHandler>();
    }

    /*
	 *
	 */
    public void StartFirstTurn()
    {
        currentToken = 0;

        photonView.RPC("RPC_StopHighlighter", RpcTarget.Others, true);
        // highlighter.StopHighlight ();

        Token.TokenType section = GetCurrentTokenType();
        photonView.RPC("RPC_Highlighter", RpcTarget.Others, section);

        photonView.RPC("RPC_ShowDice", RpcTarget.Others, section);
        // diceManager.ShowDice (GetCurrentTokenType ());

        photonView.RPC("RPC_ActiveDice", RpcTarget.Others, true);

        photonView.RPC("RPC_CurrentPlayer", RpcTarget.Others, currentToken);
        playerHandler.currentActivePlayer = tokenManager.ActivePlayers[currentToken];
        StartCoroutine(StartTurn());
    }

    /*
	 * Start Turn
	 */
    public IEnumerator StartTurn()
    {
        /*
		 * delay 
		 */
        yield return new WaitForSeconds(0.1f);
        Token.TokenPlayer tokenPlayer = GetCurrentTokenPlayer();
        highlighter.StopHighlight();
        highlighter.Highlight(GetCurrentTokenType());

        diceManager.ShowDice(GetCurrentTokenType());
        dice = diceManager.GetCurrentActiveDice();

        if (tokenPlayer == Token.TokenPlayer.Player)
        {

            dice.EnableUserInteraction = true;
        }
        else if (tokenPlayer == Token.TokenPlayer.Opponent)
        {


            yield return new WaitForSeconds(0.7f);
            dice.EnableUserInteraction = false;
            dice = diceManager.GetCurrentActiveDice();


            dice.EnableUserInteraction = true;
            tokenManager.EnableSelectionMode(GetCurrentTokenType());
 
        }
  
    }

    /*
	 * Next Turn for Player////
	 */

    public void NextTurn()
    {

        photonView.RPC("Player_CurrentToken", RpcTarget.All);
        diceSixCount = 0;
        /* int count = selectedTypes.Length;
         currentToken++;
         if (currentToken >= count)
         {
             currentToken = 0;
         }*/


        Debug.Log("ON NEXT TURN" + currentToken);
        if (onNextTurn != null)
        {
            onNextTurn(GetCurrentTokenType());

        }

        playerHandler.currentActivePlayer = tokenManager.ActivePlayers[currentToken];

        Token.TokenType section = GetCurrentTokenType();
        photonView.RPC("RPC_StopHighlighter", RpcTarget.Others, true);
         highlighter.StopHighlight ();


        photonView.RPC("RPC_Highlighter", RpcTarget.Others, section);

        photonView.RPC("RPC_ShowDice", RpcTarget.Others, section);


        photonView.RPC("RPC_ActiveDice", RpcTarget.Others, true);
        photonView.RPC("RPC_CurrentPlayer", RpcTarget.Others, currentToken);
    }

    [PunRPC]
    void Player_CurrentToken()
    {

        if (diceSixCount > 6) return;
        int count = selectedTypes.Length;
        currentToken++;
        if (currentToken >= count)
        {
            currentToken = 0;
        }
        dice = diceManager.GetCurrentActiveDice();
        dice.EnableUserInteraction = true;
        
    }
    [PunRPC]
    void RPC_CurrentPlayer(int val)
    {


        playerHandler.currentActivePlayer = tokenManager.ActivePlayers[val];

    }
    [PunRPC]
    void SetCurrentActivePlayer(Token.TokenType type)
    {



        diceManager.ShowDice(type);

    }


    [PunRPC]
    void RPC_PlayerActiveDice(Token.TokenPlayer player)
    {

        if (player == Token.TokenPlayer.Player)
        {
            diceManager.ShowDice(GetCurrentTokenType());
            Token.TokenPlayer opponent = Token.TokenPlayer.Opponent;
            photonView.RPC("RPC_PlayerInActiveDice", RpcTarget.Others, opponent);
;
        }
        if (player == Token.TokenPlayer.Opponent)
        {
            Debug.Log("OPPONENT>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            diceManager.ShowDice(GetCurrentTokenType());

        }
    }




    /*
	 * method callback. akan dipanggil ketika dadu selesai dikocok
	 * oleh DiceManager
	 */
    public void DiceRolled(int diceNum, Token.TokenType type)
    {
        if (diceNum == 6)
        {
            diceSixCount++;
            if (diceSixCount >= 3)
            {
                dice.EnableUserInteraction = false;
                diceSixCount = 0;

                if (onSixThreeTimes != null)
                {
                    onSixThreeTimes(GetCurrentTokenType());
                }

                NextTurn();
                StartCoroutine(StartTurn());
                return;
            }
        }

        lastDiceNum = diceNum;
        Token.TokenPlayer player = GetCurrentTokenPlayer();
        List<Token> movableTokens = tokenManager.GetMovableTokens(GetCurrentTokenType(), diceNum);

        if (diceNum != 6 && movableTokens.Count <= 0)
        {
            NextTurn();
            StartCoroutine(StartTurn());
            return;
        }

        if (player == Token.TokenPlayer.Player || player == Token.TokenPlayer.Opponent)
        {
            dice.EnableUserInteraction = false;

            if (movableTokens.Count == 1)
            {
                if (!movableTokens[0].MoveToken(diceNum))
                {
                    NextTurn();
                    StartCoroutine(StartTurn());
                }
            }
            else if (movableTokens.Count > 1)
            {
                for (int i = 0; i < movableTokens.Count; i++)
                    movableTokens[i].SelectionMode = true;


            }
        }
        else if (player == Token.TokenPlayer.Opponent)
        {

            //opponentCtrl.Play (diceNum);
        }

    }



    /*
	 * callback onTokenSelected  object Token
	 */

    public void TokenSelected(Token selectedToken)
    {

        Debug.Log("Selected Token :" + selectedToken);
        highlighter.StopHighlight();

        if (selectedToken == null)
        {
            NextTurn();
            StartCoroutine(StartTurn());
            return;
        }

        if (selectedToken.State == Token.TokenState.Locked)
        {
            if (lastDiceNum == 6)
            {
                selectedToken.Unlock();
            }
        }
        else
        {
            if (!selectedToken.MoveToken(lastDiceNum))
            {
                NextTurn();
                StartCoroutine(StartTurn());
            }
        }
    }

    /*
	 * 
	 */
    public void TokenAnimationsEnd()
    {
        if (lastDiceNum != 6)
        {
            NextTurn();
        }

        StartCoroutine(StartTurn());
    }

    public Token.TokenType GetCurrentTokenType()
    {
        return selectedTypes[currentToken];
    }

    public Token.TokenPlayer GetCurrentTokenPlayer()
    {
        return tokenManager.GetTokensOfType(GetCurrentTokenType())[0].GetComponent<Token>().Player;
    }


    [PunRPC]
    void RPC_StopHighlighter(bool val)
    {
        highlighter.StopHighlight();
    }

    [PunRPC]
    void RPC_Highlighter(Token.TokenType section)
    {
        highlighter.Highlight(section);
    }

    [PunRPC]
    void RPC_ShowDice(Token.TokenType section)
    {
        diceManager.ShowDice(section);
    }

    [PunRPC]
    void RPC_ActiveDice(bool val)
    {
        dice = diceManager.GetCurrentActiveDice();

    }

    [PunRPC]
    void RPC_DiceInteractable(bool val)
    {
        dice = diceManager.GetCurrentActiveDice();
        dice.EnableUserInteraction = val;

    }






}

