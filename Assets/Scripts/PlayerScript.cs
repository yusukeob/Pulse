using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private int myScore = 0;

    private List<GameObject> hand = new List<GameObject>();
    private GameObject chosenCard;

    private int playerNum;
    private int numPlayers;
    private float xStartPos = -5;
    private float xNextPos = 2;
    private float yPos = -3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayer(int[] hand, int playerNum, int numPlayers)
    {
        this.playerNum = playerNum;
        this.numPlayers = numPlayers;
        this.myScore = 0;
        foreach(GameObject card in this.hand)
        {
            Destroy(card);
        }
        this.hand.Clear();
        UpdateScore();

        GameObject cardFrontPrefab;
        GameObject playArea = GameObject.Find("InnerTable");

        for (int i = 0; i < hand.Length; i++)
        {
            cardFrontPrefab = (GameObject)Resources.Load("Prefabs/card_front_" + hand[i], typeof(GameObject));

            float xCardPos = playArea.transform.position.x + xStartPos + xNextPos * i;
            float yCardPos = playArea.transform.position.y + yPos;
            Vector3 cardPos = new Vector3(xCardPos, yCardPos, playArea.transform.position.z);

            GameObject card = Instantiate(cardFrontPrefab, cardPos, Quaternion.Euler(0, 0, 0));
            card.GetComponent<Card>().SetValue(hand[i]);
            this.hand.Add(card);
        }
    }

    public void CardChosen(GameObject chosenCard)
    {
        if (chosenCard == null)
        {
            return;
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (chosenCard == hand[i])
            {
                this.chosenCard = hand[i];
                this.chosenCard.transform.position = new Vector3(0, -1.25f, 0);
                hand.RemoveAt(i);
                break;
            }
        }
    }

    public void DestroyChosenCard()
    {
        Destroy(chosenCard);
        PositionHand();
    }

    public void ReturnChosenCard()
    {
        if (chosenCard == null)
        {
            return;
        }

        hand.Add(chosenCard);
        hand.Sort((a, b) => a.GetComponent<Card>().GetValue() - b.GetComponent<Card>().GetValue());

        PositionHand();
    }

    public void PositionHand()
    {
        GameObject playArea = GameObject.Find("InnerTable");

        for (int i = 0; i < hand.Count; i++)
        {
            float xCardPos = playArea.transform.position.x + xStartPos + xNextPos * i;
            float yCardPos = playArea.transform.position.y + yPos;
            Vector3 cardPos = new Vector3(xCardPos, yCardPos, playArea.transform.position.z);
            hand[i].transform.position = cardPos;
        }
    }

    public void ProcessZeroWin(GameObject winnerCard, int winScore)
    {
        if (chosenCard == null)
        {
            return;
        }

        if (winnerCard == chosenCard)
        {
            myScore += winScore;
            UpdateScore();
        }

        DestroyChosenCard();
    }

    public void ProcessPosWin(GameObject winnerCard, int winScore)
    {
        if (chosenCard == null)
        {
            return;
        }

        if (winnerCard == chosenCard)
        {
            myScore += winScore;
            UpdateScore();
            DestroyChosenCard();
        }
        else
        {
            if (chosenCard.GetComponent<Card>().GetValue() > 0)
            {
                ReturnChosenCard();
            }
            else
            {
                DestroyChosenCard();
            }
        }
    }

    public void ProcessNegWin(GameObject winnerCard, int winScore)
    {
        if (chosenCard == null)
        {
            return;
        }

        if (winnerCard == chosenCard)
        {
            myScore += winScore;
            UpdateScore();
            DestroyChosenCard();
        }
        else
        {
            if (chosenCard.GetComponent<Card>().GetValue() < 0)
            {
                ReturnChosenCard();
            }
            else
            {
                DestroyChosenCard();
            }
        }
    }

    public void UpdateScore()
    {
        GameObject myScoreContainer = GameObject.Find("P" + playerNum + "Score");
        Text myScoreText = myScoreContainer.GetComponent<Text>();

        myScoreText.text = "P" + playerNum +" Score: " + myScore;
    }


    public GameObject GetChosenCard()
    {
        return chosenCard;
    }

    public List<GameObject> GetHand()
    {
        return hand;
    }

    public int GetPlayerScore()
    {
        return myScore;
    }

    public int GetPlayerNum()
    {
        return playerNum;
    }
}
