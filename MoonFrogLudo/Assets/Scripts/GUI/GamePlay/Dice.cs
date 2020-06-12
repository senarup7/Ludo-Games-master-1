using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Dice : MonoBehaviourPun {

	public delegate void OnDiceRolled (int diceNum);

	public event OnDiceRolled onDiceRolled;


    public Sprite[] diceSides;



    [SerializeField] private int minDiceNumber = 1;
	[SerializeField] private int maxDiceNumber = 6;

    // Reference to sprite renderer to change sprites
    private SpriteRenderer rend;

    private Animator animator;
	private bool rolling;

	private bool enableUserInteraction;
	public bool EnableUserInteraction {
		set { enableUserInteraction = value; }
	}

	void Start () 
	{
		animator = GetComponent<Animator> ();
		rolling = false;
		enableUserInteraction = true;

        // Assign Renderer component
        rend = GetComponent<SpriteRenderer>();

        // Load dice sides sprites to array from DiceSides subfolder of Resources folder
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");


    }

	void Update ()
	{
		if (!enableUserInteraction || rolling) {


            return;
		}
        if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
        {

            //  

                if (GameMaster.instance._GameplayHandler._PlayerHandler.IsMyTurnForPVP())
                 {
                
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
                    if (hit)
                    {
                        if (hit.transform == transform)
                        {
                            Debug.Log("hit Transform ");

                            StartCoroutine(RollTheDice());

                        }
                    }
                }
           }
        }
	}


    // Coroutine that rolls the dice
    public IEnumerator RollTheDice()
    {
        if (!rolling)
        {

            rolling = true;
            rend = GetComponent<SpriteRenderer>();

            // Variable to contain random dice side number.
            // It needs to be assigned. Let it be 0 initially
            int randomDiceSide = 0;

            // Final side or value that dice reads in the end of coroutine
            int finalSide = 0;

            // Loop to switch dice sides ramdomly
            // before final side appears. 20 itterations here.
            for (int i = 0; i <= 10; i++)
            {

              
                // Pick up random value from 0 to 5 (All inclusive)
                randomDiceSide =  Random.Range(0, 6);

                // Set sprite to upper face of dice from array according to random value
                rend.sprite = diceSides[randomDiceSide];
                if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
                    photonView.RPC("PVP_OnDiceRollComplete", RpcTarget.Others, randomDiceSide);
                // Pause before next itteration
                yield return new WaitForSeconds(0.1f);
            }
           
            // Assigning final side so you can use this value later in your game
            // for player movement for example
            finalSide = randomDiceSide + 1;
           
            if (onDiceRolled != null)
            {
                onDiceRolled(finalSide);

            }
            if (GameMaster.instance._GamePlayType == GamePlayType.ONLINE)
                photonView.RPC("PVP_OnDiceRollComplete", RpcTarget.Others, randomDiceSide);
            // Show final dice value in Console
            Debug.Log("finalSide "+ finalSide);
            rolling = false;
            yield return null;
        }
    }
    [PunRPC]
    public void PVP_OnDiceRollComplete(int diceValue)
    {
        UpdateDiceImage(diceValue);
    }
    public void UpdateDiceImage(int diceValue)
    {

        rend = GetComponent<SpriteRenderer>();
        rend.sprite = diceSides[diceValue];
        // onDiceRolled(diceValue);
    }
    public IEnumerator RollDice ()
	{
		if (!rolling) {

            Debug.Log("Dice Roll");
			rolling = true;
			animator.SetTrigger ("RollDice");
			animator.SetInteger ("DiceNum", 0);
			yield return new WaitForSeconds (1f);

			rolling = false;
			int num = Random.Range (minDiceNumber, maxDiceNumber + 1);
		//	animator.SetInteger ("DiceNum", num);
			
			if (onDiceRolled != null) {
				onDiceRolled (num);
			}
		}

		yield return null;
	}
}
