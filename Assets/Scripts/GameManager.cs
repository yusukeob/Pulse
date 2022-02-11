using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<GameObject> players;
    private int numHumans;
    private int numAI;
    private GameObject gameTextContainer;
    private Text gameText;
    private GameMode gameMode;
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
        GameObject.Find("StartButton").SetActive(false);
        players = GameObject.Find("ObjectContainer").GetComponent<GameComponents>().DealDeck(numHumans, numAI);

        gameText.text = "Choose Card";
        gameTextContainer.SetActive(true);

        gameMode = GameMode.ChooseCard;
    }

    public GameMode GetGameMode()
    {
        return this.gameMode;
    }
}
