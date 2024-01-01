using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogicScript : MonoBehaviour
{
    public GameObject player;
    public Text scorePlayer1;
    public Text scorePlayer2;
    // Start is called before the first frame update
    void Start()
    {
        //var p = Instantiate(player);

    }

    // Update is called once per frame
    void Update()
    {
        scorePlayer1.text = Utils.playerOneScore.ToString();
        scorePlayer2.text = Utils.playerTwoScore.ToString();
        
    }
}
