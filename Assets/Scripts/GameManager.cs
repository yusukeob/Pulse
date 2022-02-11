using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int numHumans;
    private int numAI;

    // Start is called before the first frame update
    void Start()
    {
        numHumans = 1;
        numAI = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        GameObject startButton = GameObject.Find("StartButton");
        startButton.SetActive(false);

        GameComponents gameComonents = GameObject.Find("ObjectContainer").GetComponent<GameComponents>();
        gameComonents.DealDeck(numHumans, numAI);
    }
}
