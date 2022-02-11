using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentScript : MonoBehaviour
{
    private int playerNum;
    private int numPlayers;
    private GameObject chosenCard;

    private List<GameObject> hand = new List<GameObject>();
    private readonly float nextPos = -0.5f;
    private readonly float xPos2 = -6.75f;
    private readonly float yPos2 = 1.3f;
    private readonly float xPos3 = -3.5f;
    private readonly float yPos3 = 3.25f;
    private readonly float xPos4 = 1.25f;
    private readonly float yPos4 = 3.25f;
    private readonly float xPos5 = 6f;
    private readonly float yPos5 = 3.25f;
    private readonly float xPos6 = 6.75f;
    private readonly float yPos6 = 1.3f;
    private readonly float rotateAngle2 = -90;
    // private readonly float rotateAngle3 = 135;
    private readonly float rotateAngle4 = 180;
    // private readonly float rotateAngle5 = 225;
    private readonly float rotateAngle6 = -270;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAI(int[] hand, int playerNum, int numPlayers)
    {
        this.playerNum = playerNum;
        this.numPlayers = numPlayers;

        GameObject playArea = GameObject.Find("InnerTable");
        GameObject cardBackPrefab = (GameObject)Resources.Load("Prefabs/card_back", typeof(GameObject)); ;

        float xStartPos = 0;
        float yStartPos = 0;
        float xNextPos = 0;
        float yNextPos = 0;
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (numPlayers == 6)
        {
            switch(playerNum)
            {
                case 2:
                    xStartPos = xPos2;
                    yStartPos = yPos2;
                    xNextPos = 0;
                    yNextPos = nextPos;
                    rotation = Quaternion.Euler(0, 0, rotateAngle2);
                    break;
                case 3:
                    xStartPos = xPos3;
                    yStartPos = yPos3;
                    xNextPos = nextPos;
                    yNextPos = 0;
                    rotation = Quaternion.Euler(0, 0, rotateAngle4);
                    break;
                case 4:
                    xStartPos = xPos4;
                    yStartPos = yPos4;
                    xNextPos = nextPos;
                    yNextPos = 0;
                    rotation = Quaternion.Euler(0, 0, rotateAngle4);
                    break;
                case 5:
                    xStartPos = xPos5;
                    yStartPos = yPos5;
                    xNextPos = nextPos;
                    yNextPos = 0;
                    rotation = Quaternion.Euler(0, 0, rotateAngle4);
                    break;
                case 6:
                    xStartPos = xPos6;
                    yStartPos = yPos6;
                    xNextPos = 0;
                    yNextPos = nextPos;
                    rotation = Quaternion.Euler(0, 0, rotateAngle6);
                    break;
            }

        }

        for (int i = 0; i < hand.Length; i++)
        {
            float xCardPos = playArea.transform.position.x + xStartPos + xNextPos * i;
            float yCardPos = playArea.transform.position.y + yStartPos + yNextPos * i;
            Vector3 cardPos = new Vector3(xCardPos, yCardPos, playArea.transform.position.z);

            GameObject card = Instantiate(cardBackPrefab, cardPos, rotation);
            card.GetComponent<Card>().SetValue(hand[i]);
            this.hand.Add(card);
        }
    }

    public void ChooseCard()
    {
        // Debug.Log("AI Player");
        int handSize = hand.Count;
        int chooseCardIdx = Random.Range(0, handSize);

        chosenCard = hand[chooseCardIdx];
        hand.RemoveAt(chooseCardIdx);

        if (numPlayers == 6)
        {
            switch (playerNum)
            {
                case 2:
                    chosenCard.transform.position = new Vector3(xPos2 + 1.75f, yPos2 - 1.75f, 0);
                    break;
                case 3:
                    chosenCard.transform.position = new Vector3(xPos3, yPos3 - 1.75f, 0);
                    break;
                case 4:
                    chosenCard.transform.position = new Vector3(xPos4 - 1.25f, yPos4 - 1.75f, 0);
                    break;
                case 5:
                    chosenCard.transform.position = new Vector3(xPos5 -2.5f, yPos5 - 1.75f, 0);
                    break;
                case 6:
                    chosenCard.transform.position = new Vector3(xPos6 - 1.75f, yPos6 - 1.75f, 0);
                    break;
            }
        }
    }
}
