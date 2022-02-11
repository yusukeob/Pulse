using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static List<GameObject> players;
    private int numHumans;
    private int numAI;
    public static GameObject revealCardsButton;
    private static GameObject gameTextContainer;
    private static Text gameText;
    private static GameMode gameMode;
    public enum GameMode
    {
        Start = 0,
        ChooseCard,
        RevealCards,
        CollectCards,
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMode = GameMode.Start;
        numHumans = 1;
        numAI = 5;
        revealCardsButton = GameObject.Find("RevealCardsButton");
        gameTextContainer = GameObject.Find("GameText");
        gameText = gameTextContainer.GetComponent<Text>();
        gameTextContainer.SetActive(false);
        GameObject.Find("RevealCardsButton").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        GameObject.Find("StartButton").SetActive(false);
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

    public static GameMode GetGameMode()
    {
        return gameMode;
    }
}
