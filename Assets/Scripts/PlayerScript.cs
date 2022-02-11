using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private List<GameObject> hand = new List<GameObject>();

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
        GameObject playArea = GameObject.Find("InnerTable");
        float xStartPos = -5;
        float xNextPos = 2;
        float yPos = -3;

        GameObject cardFrontPrefab;
        //int handSize = hand.Length;

        for (int i = 0; i < hand.Length; i++)
        {
            cardFrontPrefab = (GameObject)Resources.Load("Prefabs/card_front_" + hand[i], typeof(GameObject));

            float xCardPos = playArea.transform.position.x + xStartPos + xNextPos * i;
            float yCardPos = playArea.transform.position.y + yPos;
            Vector3 cardPos = new Vector3(xCardPos, yCardPos, playArea.transform.position.z);

            this.hand.Add(Instantiate(cardFrontPrefab, cardPos, Quaternion.Euler(0,0,0)));
        }
    }
}
