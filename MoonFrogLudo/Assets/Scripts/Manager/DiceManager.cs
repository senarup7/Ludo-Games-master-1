using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{

    /*
	 * onDiceRolled event 
	 */
    public delegate void OnDiceRolled(int diceNum, Token.TokenType type);
    public event OnDiceRolled onDiceRolled;

    public GameObject[] placeHolders;
    public GameObject[] dices;
    public Token.TokenType activeDice = Token.TokenType.Blue;

 /// <summary>
 /// 
 /// </summary>
 /// <param name="selected"></param>
    public void Init(Token.TokenType[] selected)
    {
        DeactivateAllDice();

        int count = Mathf.Min(selected.Length, 4);
        for (int i = 0; i < count; i++)
        {
            int idx = (int)selected[i];
            placeHolders[idx].SetActive(true);
        }
    }

  
    public void ShowDice(Token.TokenType type)
    {

 
        dices[(int)activeDice].SetActive(false);
        dices[(int)type].SetActive(true);
        activeDice = type;
    }

    public Dice GetCurrentActiveDice()
    {
        return dices[(int)activeDice].GetComponent<Dice>();
    }

    void Start()
    {

        placeHolders = new GameObject[4];
        placeHolders[0] = GameObject.FindWithTag("BlueDicePlaceHolder");
        placeHolders[3] = GameObject.FindWithTag("YellowDicePlaceHolder");
        placeHolders[1] = GameObject.FindWithTag("RedDicePlaceHolder");
        placeHolders[2] = GameObject.FindWithTag("GreenDicePlaceHolder");


        dices = new GameObject[placeHolders.Length];
        dices[0] = FindDiceFrom(placeHolders[0], "DiceBlue");
        dices[3] = FindDiceFrom(placeHolders[3], "DiceYellow");
        dices[1] = FindDiceFrom(placeHolders[1], "DiceRed");
        dices[2] = FindDiceFrom(placeHolders[2], "DiceGreen");


        for (int i = 0; i < dices.Length; i++)
        {
            Dice dice = dices[i].GetComponent<Dice>();
            if (dice != null)
            {
               
                dice.onDiceRolled += DiceRolled;
            }
        }

        DeactivateAllDice();
    }

    GameObject FindDiceFrom(GameObject parent, string diceName)
    {
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.name == diceName)
            {
                return t.GetChild(i).gameObject;
            }
        }

        return null;
    }

    void DiceRolled(int diceNum)
    {
        if (onDiceRolled != null)
        {
            onDiceRolled(diceNum, activeDice);
        }
    }

    public void DeactivateAllDice()
    {
        for (int i = 0; i < placeHolders.Length; i++)
        {
            placeHolders[i].SetActive(false);
            dices[i].SetActive(false);
        }
    }
    public void DisableAllDice()
    {
        for (int i = 0; i < placeHolders.Length; i++)
        {
            //   placeHolders[i].SetActive(false);
            dices[i].GetComponent<Dice>().EnableUserInteraction = false;
        }
    }


}
