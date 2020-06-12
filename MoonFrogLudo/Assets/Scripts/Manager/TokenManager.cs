
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TokenManager : MonoBehaviour
{

    public delegate void OnTokenAnimationsDone();

    public event OnTokenAnimationsDone onTokenAnimationsDone;

    [SerializeField] private GameObject playerBluePrefab;
    [SerializeField] private GameObject playerRedPrefab;
    [SerializeField] private GameObject playerGreenPrefab;
    [SerializeField] private GameObject playerYellowPrefab;

    private WaypointManager waypointManager;
    private HomeBaseManager homeBaseManager;
    public List<GameObject> Players = new List<GameObject>();
    public List<GameObject> ActivePlayers = new List<GameObject>();
    public List<GameObject> TokenList = new List<GameObject>();
    [SerializeField] private GameObject TokenParent;
    private Dictionary<Token.TokenType, Token[]> tokens;
    private List<Token> tokenComps;

    public Dictionary<Token.TokenType, Token[]> Tokens
    {
        get { return tokens; }
    }

    public Token[] GetTokensOfType(Token.TokenType type)
    {
        return Tokens[type];
    }

    public List<Token> GetLockedTokens(Token.TokenType type)
    {
        List<Token> lockedTokens = new List<Token>();
        Token[] tokensOfType = GetTokensOfType(type);
        for (int i = 0; i < tokensOfType.Length; i++)
        {
            if (tokensOfType[i].State == Token.TokenState.Locked)
                lockedTokens.Add(tokensOfType[i]);
        }

        return lockedTokens;
    }

    public List<Token> GetUnlockedTokens(Token.TokenType type)
    {
        List<Token> lockedTokens = new List<Token>();
        Token[] tokensOfType = GetTokensOfType(type);
        for (int i = 0; i < tokensOfType.Length; i++)
        {
            if (tokensOfType[i].State != Token.TokenState.Locked &&
                tokensOfType[i].State != Token.TokenState.Finish)
            {

                lockedTokens.Add(tokensOfType[i]);
            }
        }

        return lockedTokens;
    }

    public List<Token> GetMovableTokens(Token.TokenType type, int diceNum)
    {
        List<Token> movableTokens = new List<Token>();
        Token[] tokensOfType = GetTokensOfType(type);
        for (int i = 0; i < tokensOfType.Length; i++)
        {
            Token currToken = tokensOfType[i];

            if (currToken.State == Token.TokenState.Locked)
            {
                if (diceNum == 6)
                    movableTokens.Add(currToken);
            }
            else if (currToken.State != Token.TokenState.Finish && currToken.IsValidMove(diceNum))
            {
                movableTokens.Add(currToken);
            }
        }

        return movableTokens;
    }

    /// <summary>
    /// EnableSelectionMode 
    /// </summary>
    /// <param name="type"></param>

    public void EnableSelectionMode(Token.TokenType type)
    {
        SetSelectionModeEnable(type, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waypointManager"></param>
    /// <param name="hbManager"></param>
    /// <param name="selected"></param>
    /// <param name="playerTypes"></param>
    public void Init(
        WaypointManager waypointManager, HomeBaseManager hbManager,
        Token.TokenType[] selected, Token.TokenPlayer[] playerTypes)
    {
        if (selected.Length == playerTypes.Length && selected.Length <= 4)
        {
            this.waypointManager = waypointManager;
            this.homeBaseManager = hbManager;

            tokens = new Dictionary<Token.TokenType, Token[]>();

            for (int i = 0; i < selected.Length; i++)
            {
                CreateTokens(selected[i], playerTypes[i]);
            }

            tokenComps = new List<Token>();
            foreach (KeyValuePair<Token.TokenType, Token[]> entry in tokens)
            {

                for (int i = 0; i < entry.Value.Length; i++)
                {
                    Token token = entry.Value[i];
                    token.onStateChanged += TokenStateChanged;
                    token.onTokenSelected += TokenSelected;
                    tokenComps.Add(token);
                }

            }

        }

    }

    void Start()
    {
    }

    void TokenStateChanged(Token.TokenState lastState, Token.TokenState newState)
    {
        if (onTokenAnimationsDone != null)
        {
            bool isDone =
                newState == Token.TokenState.Idle || newState == Token.TokenState.Locked ||
                newState == Token.TokenState.Finish;
            isDone = isDone && !AnyAnimatingToken();
            if (isDone)
            {
                onTokenAnimationsDone();
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedToken"></param>
    void TokenSelected(Token selectedToken)
    {
        SetSelectionModeEnable(selectedToken.Type, false);
    }

    void SetSelectionModeEnable(Token.TokenType type, bool enable)
    {
        Token[] currTokens = GetTokensOfType(type);
        for (int i = 0; i < currTokens.Length; i++)
            currTokens[i].SelectionMode = enable;
    }

    /*
	 * 
	 */
    bool AnyAnimatingToken()
    {
        foreach (Token token in tokenComps)
        {
            if (token.IsAnimating())
            {
                return true;
            }
        }
        return false;
    }
    int tempindex = 0;
    void CreateTokens(Token.TokenType tokenType, Token.TokenPlayer playerType)
    {

        Transform[] homeBase = null;
        GameObject tokenPrefab = null;
        string pname = "";
        List<Transform> waypoint = null;

        switch (tokenType)
        {
            case Token.TokenType.Blue:
                homeBase = homeBaseManager.BlueHomeBase;

                tokenPrefab = playerBluePrefab;
                waypoint = waypointManager.BlueWaypoints;
                pname = tokenPrefab.name;

                break;
            case Token.TokenType.Red:
                homeBase = homeBaseManager.RedHomeBase;
                tokenPrefab = playerRedPrefab;
                waypoint = waypointManager.RedWaypoints;
                pname = tokenPrefab.name;
                break;
            case Token.TokenType.Green:
                homeBase = homeBaseManager.GreenHomeBase;
                tokenPrefab = playerGreenPrefab;
                waypoint = waypointManager.GreenWaypoints;
                pname = tokenPrefab.name;
                break;
            case Token.TokenType.Yellow:
                homeBase = homeBaseManager.YellowHomeBase;
                tokenPrefab = playerYellowPrefab;
                waypoint = waypointManager.YellowWaypoints;
                pname = tokenPrefab.name;
                break;
        }

        Token[] currTokens = new Token[homeBase.Length];
        tokens.Add(tokenType, currTokens);
        if (TokenParent.transform.childCount >= 8)
        {
            return;
        }

        for (int i = 0; i < homeBase.Length; i++)
        {
            TokenList[tempindex].gameObject.SetActive(true);

            Token token = TokenList[tempindex].GetComponent<Token>();
            switch (token.Type)
            {
                case Token.TokenType.Blue:
                    token.WayPointTransform = waypointManager.blueWaypoints;

                    break;
                case Token.TokenType.Yellow:
                    token.WayPointTransform = waypointManager.yellowWaypoints;
                    break;
                case Token.TokenType.Red:
                    token.WayPointTransform = waypointManager.redWaypoints;
                    break;
                case Token.TokenType.Green:
                    token.WayPointTransform = waypointManager.greenWaypoints;
                    break;
            }
            token = token.GetComponent<Token>();
            token.transform.position = homeBase[i].position;
 
            token.SetupWaypoints(waypoint, homeBase[i].transform, i);
            token.Player = playerType;

            currTokens[i] = token;
            tempindex++;
        }


        /*    	for (int i = 0; i < homeBase.Length; i++) {
        			GameObject newTokenGO = PhotonNetwork.Instantiate (tokenPrefab.name, homeBase [i].position, Quaternion.identity);
                //   Players.Add(newTokenGO);
                   Token token = newTokenGO.GetComponent<Token> ();
        		    token.SetupWaypoints (waypoint, homeBase[i]);
        		    token.Player = playerType;

        			currTokens [i] = token;
                   // Transform t= Players[tempindex].transform;
                     newTokenGO.transform.SetParent(TokenParent.transform);

        		}*/


    }


}

