using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static List<GameObject> players;
    private int numHumans;
    private int numAI;
    private int carryOverPoints = 0;
    public static GameObject startButton;
    public static GameObject revealCardsButton;
    public static GameObject processRoundButton;
    private static GameObject gameTextContainer;
    private static Text gameText;
    private static GameMode gameMode;
    public enum GameMode
    {
        Start = 0,
        ChooseCard,
        RevealCards,
        ProcessRound,
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMode = GameMode.Start;

        numHumans = 1;
        numAI = 5;

        startButton = GameObject.Find("StartButton");
        revealCardsButton = GameObject.Find("RevealCardsButton");
        revealCardsButton.SetActive(false);
        processRoundButton = GameObject.Find("ProcessRoundButton");
        processRoundButton.SetActive(false);
        gameTextContainer = GameObject.Find("GameText");
        gameText = gameTextContainer.GetComponent<Text>();
        gameTextContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        startButton.SetActive(false);
        players = GameObject.Find("ObjectContainer").GetComponent<GameComponents>().DealDeck(numHumans, numAI);

        gameText.text = "Choose Card";
        gameTextContainer.SetActive(true);

        gameMode = GameMode.ChooseCard;
    }

    public static void OnCardChosen(GameObject chosenCard)
    {
        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                player.GetComponent<PlayerScript>().CardChosen(chosenCard);
            }
            else if(player.tag == "AiPlayer")
            {
                player.GetComponent<OpponentScript>().ChooseCard();
            }
        }

        gameText.text = "";
        gameTextContainer.SetActive(false);
        gameMode = GameMode.RevealCards;
        revealCardsButton.SetActive(true);
    }

    public void RevealCards()
    {
        revealCardsButton.SetActive(false);

        foreach (GameObject player in players)
        {
            if (player.tag == "AiPlayer")
            {
                player.GetComponent<OpponentScript>().RevealCard();
            }
        }

        gameMode = GameMode.ProcessRound;
        processRoundButton.SetActive(true);
    }

    public void ProcessRound()
    {
        processRoundButton.SetActive(false);

        CalcRoundResults();

        gameMode = GameMode.ChooseCard;
        gameText.text = "Choose Card";
        gameTextContainer.SetActive(true);
    }

    public void CalcRoundResults()
    {
        List<GameObject> chosenCards = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                chosenCards.Add(player.GetComponent<PlayerScript>().GetChosenCard());
            }
            else if (player.tag == "AiPlayer")
            {
                chosenCards.Add(player.GetComponent<OpponentScript>().GetChosenCard());
            }
        }

        int negativeSum = 0;
        int positiveSum = 0;
        int maxNegativeValue = 0;
        int maxPositiveValue = 0;
        GameObject maxNegativeCard = new GameObject();
        GameObject maxPositiveCard = new GameObject();
        List<GameObject> zeroCards = new List<GameObject>();
        int numZeros = 0;
        foreach(GameObject chosenCard in chosenCards)
        {
            int cardValue = chosenCard.GetComponent<Card>().GetValue();
            if (cardValue == 0)
            {
                numZeros++;
                zeroCards.Add(chosenCard);
            }
            else if (cardValue > 0)
            {
                positiveSum += cardValue;
                if (cardValue > maxPositiveValue)
                {
                    maxPositiveValue = cardValue;
                    maxPositiveCard = chosenCard;
                }
            }
            else if (cardValue < 0)
            {
                negativeSum += cardValue;
                if (cardValue < maxNegativeValue)
                {
                    maxNegativeValue = cardValue;
                    maxNegativeCard = chosenCard;
                }

            }
        }

        // victory conditions
        GameObject winnerCard = null;
        bool hasWinner = false;
        bool zeroWin = false;
        bool posWin = false;
        bool negWin = false;
        if(positiveSum == Mathf.Abs(negativeSum) || positiveSum == 0 || negativeSum == 0)
        {
            if (numZeros == 1)
            {
                winnerCard = zeroCards[0];
                hasWinner = true;
                zeroWin = true;
            }
        }
        else if(positiveSum > Mathf.Abs(negativeSum))
        {
            winnerCard = maxPositiveCard;
            hasWinner = true;
            posWin = true;
        }
        else if(Mathf.Abs(negativeSum) > positiveSum)
        {
            winnerCard = maxNegativeCard;
            hasWinner = true;
            negWin = true;
        }

        /*if (!hasWinner)
        {
            carryOverPoints += (positiveSum += Mathf.Abs(negativeSum));
        }*/

        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                if (!hasWinner)
                {
                    //player.GetComponent<PlayerScript>().DestroyChosenCard();
                    player.GetComponent<PlayerScript>().ReturnChosenCard();
                }
                else if (zeroWin)
                {
                    player.GetComponent<PlayerScript>().ProcessZeroWin(winnerCard, positiveSum + Mathf.Abs(negativeSum) + carryOverPoints);
                }
                else if (posWin)
                {
                    player.GetComponent<PlayerScript>().ProcessPosWin(winnerCard, Mathf.Abs(negativeSum) + carryOverPoints);
                }
                else if (negWin)
                {
                    player.GetComponent<PlayerScript>().ProcessNegWin(winnerCard, positiveSum + carryOverPoints);
                }
            }
            else if (player.tag == "AiPlayer")
            {
                if (!hasWinner)
                {
                    // player.GetComponent<OpponentScript>().DestroyChosenCard();
                    player.GetComponent<OpponentScript>().ReturnChosenCard();
                }
                else if (zeroWin)
                {
                    player.GetComponent<OpponentScript>().ProcessZeroWin(winnerCard, positiveSum + Mathf.Abs(negativeSum));
                }
                else if (posWin)
                {
                    player.GetComponent<OpponentScript>().ProcessPosWin(winnerCard, Mathf.Abs(negativeSum) + carryOverPoints);
                }
                else if (negWin)
                {
                    player.GetComponent<OpponentScript>().ProcessNegWin(winnerCard, positiveSum + carryOverPoints);
                }
            }
        }

        /*if (hasWinner)
        {
            carryOverPoints = 0;
        }*/
    }

    public static GameMode GetGameMode()
    {
        return gameMode;
    }
}
