using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLine : MonoBehaviour
{
    //Array met planeten met als de eerste 4 de planeten links het dichts bij de player index 5 in de centrale planeet en de laatste 4 zijn planeten het dichts bij de enemy kant
    [SerializeField]
    GameObject[] planets;

    //enemy en player line renderer
    [SerializeField]
    LineRenderer EnemyLine;
    [SerializeField]
    LineRenderer PlayerLine;

    //hoeveel games/gevechten er zijn gespeeld
    [SerializeField]
    int game = 0;
    

    //start locatie enemy en player (ook van de lijn)
    [SerializeField]
    Transform playerStart;
    [SerializeField]
    Transform EnemyStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderLines();
    }

    /// <summary>
    /// Renderd het eerste punt van de line en drawt de totale line
    /// </summary>
    void RenderLines()
    {
        PlayerLine.positionCount = game + 1;
        EnemyLine.positionCount = game + 1;

        PlayerLine.SetPosition(0, playerStart.position);
        EnemyLine.SetPosition(0, EnemyStart.position);

        DrawLine(game + 1);
    }

    /// <summary>
    /// renderd de lijn tot bij welke game je bent (als je bij de eerste game bent gaat de lijn tot de eerste planeer ben je bij game 2 gaat ie naar de 2e etc. je ziet de veranderingen als je realtime de game integer aanpast in game)
    /// </summary>
    /// <param name="amount">hoevaak de for loop moet loopen dus het aantal games wat is gespeeld plus de player en enemy locatie dus is altijd minimaal 1</param>
    void DrawLine(int amount)
    {
        if (amount >= 2)
        {
            for (int i = 0; i < amount; i++)
            {
                PlayerLine.SetPosition(i, planets[i].transform.position);
                EnemyLine.SetPosition(i, planets[8 - i].transform.position);
            }
        }
    }
}
