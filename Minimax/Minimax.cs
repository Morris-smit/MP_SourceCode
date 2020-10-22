using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax : MonoBehaviour
{
    //make a simulation of all possible gamestates(moves for each ship)
    public static int Evaluation(GameState gameState, bool isMaximizing)
    {
        if (isMaximizing)
        {
            //game state has the data of the game stored in it
            Ship[] pShipList = gameState.getPShipList();
            Ship[] cShipList = gameState.getCShipList();

            //this needs to happen when before calling evaluation
            //check for each ship
            foreach (Ship s in pShipList)
            {
                Ship _ship = s;

                for (int i = 0; i < 3; i++)
                {
                    s.state = Ship.State.defend;
                }
            }
        }
        


        return 1;
    }
}
