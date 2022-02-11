using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameComponents : MonoBehaviour
{
    private static int[] deck = new int[] {
         0,  0,  0,  0,  0,  0,
         1,  2,  3,  4,  5,  6,  7,  8,  9,  10,  11,  12,  13,  14,  15,
        -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15
    };
    private static int deckSize;

    // Start is called before the first frame update
    void Start()
    {
        deckSize = deck.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> DealDeck(int numHumans, int numAI)
    {
        List<GameObject> players = new List<GameObject>();

        int[] dealDeck = ShuffleDeck();
        int numPlayers = numHumans + numAI;
        int handSize = deckSize / numPlayers;
        

        for (int i = 0; i < (numHumans + numAI); i++)
        {
            int[] playerHand = dealDeck.Skip(i * handSize).Take(handSize).OrderBy(i => i).ToArray();
            if (i < numHumans)
            {
                GameObject humanPrefab = (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
                GameObject humanPlayer = Instantiate(humanPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                humanPlayer.GetComponent<PlayerScript>().CreatePlayer(playerHand, i + 1, numPlayers);
                players.Add(humanPlayer);
            }
            else
            {
                GameObject aiPrefab = (GameObject)Resources.Load("Prefabs/Opponent", typeof(GameObject));
                GameObject aiPlayer = Instantiate(aiPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                aiPrefab.GetComponent<OpponentScript>().CreateAI(playerHand, i + 1, numPlayers);
                players.Add(aiPlayer);
            }
        }

        return players;
    }

    private int[] ShuffleDeck()
    {
        int[] shuffleDeck = (int[])deck.Clone();
        System.Random random = new System.Random();
        shuffleDeck = shuffleDeck.OrderBy(x => random.Next()).ToArray();

        return shuffleDeck;
    }
}
