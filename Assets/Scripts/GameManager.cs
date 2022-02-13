using System;
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
    private static GameObject  gameEndTextContainer;
    private static Text gameEndText;

    private static GameMode gameMode;
    public enum GameMode
    {
        Start = 0,
        ChooseCard,
        RevealCards,
        ProcessRound,
        GameEnd,
    }
    private static GameEndCondition gameEndCondition;
    public enum GameEndCondition
    {
         LessThanTwoPlayersLeft = 0,
         OnlyPositiveCardsLeft,
         OnlyNegativeCardsLeft,
         StaleMate,
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
        gameEndTextContainer = GameObject.Find("GameEndText");
        gameEndText = gameEndTextContainer.GetComponent<Text>();
        gameEndTextContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMode == GameMode.GameEnd)
        {
            ProcessGameEnd();
        }
    }

    public void StartGame()
    {
        //ResetGame

        gameEndTextContainer.SetActive(false);
        gameEndText.text = "";

        startButton.SetActive(false);
        players = GameObject.Find("ObjectContainer").GetComponent<GameComponents>().DealDeck(numHumans, numAI);

        gameText.text = "Choose Card";
        gameTextContainer.SetActive(true);

        gameMode = GameMode.ChooseCard;
    }

    public void ProcessGameEnd()
    {
        //Display winner and game end
        Tuple<bool, int, int> winnerPlayerNumScore =  DetermineWinnerPlayerNumScore();
        bool isTie = winnerPlayerNumScore.Item1;
        int winnerPlayerNum = winnerPlayerNumScore.Item2;
        int winnerScore = winnerPlayerNumScore.Item3;


        string gameEndDisplayText = "Game End: ";
        if (isTie)
        {
            gameEndDisplayText += "Tie";
        }
        else
        {
            gameEndDisplayText += "Winner Player " + winnerPlayerNum + ", Score " + winnerScore;
        }
        switch (gameEndCondition)
        {
            case GameEndCondition.OnlyPositiveCardsLeft:
                gameEndDisplayText += ", Only Positive Cards Remaining";
                break;
            case GameEndCondition.OnlyNegativeCardsLeft:
                gameEndDisplayText += ", Only Negative Cards Remaining";
                break;
            case GameEndCondition.StaleMate:
                gameEndDisplayText += ", Stalemate";
                break;
        }
        gameEndText.text = gameEndDisplayText;
        gameEndTextContainer.SetActive(true);
        
        gameMode = GameMode.Start;
        startButton.GetComponentInChildren<Text>().text = "Next Game";
        startButton.SetActive(true);
    }

    public Tuple<bool, int, int> DetermineWinnerPlayerNumScore()
    {
        int maxScore = 0;
        int winnerPlayerNum = 0;
        bool isTie = false;
        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                int playerScore = player.GetComponent<PlayerScript>().GetPlayerScore();
                if (playerScore > maxScore)
                {
                    maxScore = playerScore;
                    winnerPlayerNum = player.GetComponent<PlayerScript>().GetPlayerNum();
                    isTie = false;
                }
                else if (playerScore == maxScore)
                {
                    isTie = true;
                }
            }
            else if (player.tag == "AiPlayer")
            {
                int playerScore = player.GetComponent<OpponentScript>().GetPlayerScore();
                if (playerScore > maxScore)
                {
                    maxScore = playerScore;
                    winnerPlayerNum = player.GetComponent<OpponentScript>().GetPlayerNum();
                    isTie = false;
                }
                else if (playerScore == maxScore)
                {
                    isTie = true;
                }
            }
        }

        return Tuple.Create(isTie, winnerPlayerNum, maxScore);
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
        bool gameEnd = CheckForGameEnd();
        if (gameEnd)
        {
            gameMode = GameMode.GameEnd;
            return;
        }

        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                List<GameObject> playerHand = player.GetComponent<PlayerScript>().GetHand();
                if (playerHand.Count == 0)
                {
                    OnCardChosen(null);
                    return;
                }
            }
        }

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
                GameObject chosenCard = player.GetComponent<PlayerScript>().GetChosenCard();
                if (chosenCard != null)
                {
                    chosenCards.Add(chosenCard);
                }
            }
            else if (player.tag == "AiPlayer")
            {
                GameObject chosenCard = player.GetComponent<OpponentScript>().GetChosenCard();
                if (chosenCard != null) {
                    chosenCards.Add(chosenCard);
                }
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
        int winnerCardValue = 0;
        if (hasWinner)
        {
            winnerCardValue = Mathf.Abs(winnerCard.GetComponent<Card>().GetValue());
        }

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
                    player.GetComponent<PlayerScript>().ProcessPosWin(winnerCard, winnerCardValue + Mathf.Abs(negativeSum) + carryOverPoints);
                }
                else if (negWin)
                {
                    player.GetComponent<PlayerScript>().ProcessNegWin(winnerCard, winnerCardValue + positiveSum + carryOverPoints);
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
                    player.GetComponent<OpponentScript>().ProcessPosWin(winnerCard, winnerCardValue + Mathf.Abs(negativeSum) + carryOverPoints);
                }
                else if (negWin)
                {
                    player.GetComponent<OpponentScript>().ProcessNegWin(winnerCard, winnerCardValue + positiveSum + carryOverPoints);
                }
            }
        }

        /*if (hasWinner)
        {
            carryOverPoints = 0;
        }*/
    }

    public bool CheckForGameEnd()
    {
        bool isGameEnd = false;

        List<List<GameObject>> playerHands = new List<List<GameObject>>();
        foreach (GameObject player in players)
        {
            if (player.tag == "HumanPlayer")
            {
                playerHands.Add(player.GetComponent<PlayerScript>().GetHand());
            }
            else if (player.tag == "AiPlayer")
            {
                playerHands.Add(player.GetComponent<OpponentScript>().GetHand());
            }
        }

        List<int> cardValuesLeft = new List<int>();
        int playersLeft = 0;
        foreach (List<GameObject> hand in playerHands)
        {
            if (hand.Count > 0)
            {
                playersLeft++;
            }
            foreach (GameObject card in hand)
            {
                cardValuesLeft.Add(card.GetComponent<Card>().GetValue());
            }
        }

        if (playersLeft <= 1)
        {
            isGameEnd = true;
            gameEndCondition = GameEndCondition.LessThanTwoPlayersLeft;
            return isGameEnd;
        }
        bool isAllNeg = true;
        bool isAllPos = true;
        int sumValuesCardsLeft = 0;
        foreach(int value in cardValuesLeft)
        {
            if (value > 0)
            {
                isAllNeg = false;
            }
            if (value < 0)
            {
                isAllPos = false;
            }
            sumValuesCardsLeft += value;
        }
        if (isAllNeg)
        {
            isGameEnd = true;
            gameEndCondition = GameEndCondition.OnlyNegativeCardsLeft;
            return isGameEnd;
        }
        else if (isAllPos)
        {
            isGameEnd = true;
            gameEndCondition = GameEndCondition.OnlyPositiveCardsLeft;
            return isGameEnd;
        }
        else if (sumValuesCardsLeft == 0 && cardValuesLeft.Count == playersLeft)
        {
            isGameEnd = true;
            gameEndCondition = GameEndCondition.StaleMate;
            return isGameEnd;
        }

        return isGameEnd;
    }

    public static GameMode GetGameMode()
    {
        return gameMode;
    }
}
