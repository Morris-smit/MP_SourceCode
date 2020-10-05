using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class SuperScene : MonoBehaviour
{
    protected Turnmanager tm;

    protected Player p;
    protected EnemyAi c;

    [SerializeField]
    protected List<GameObject> pShipsToLoad;
    [SerializeField]
    protected List<GameObject> cShipsToLoad;

    protected Transform[] PShipPos;
    protected Transform[] CShipPos;

    [Header("playerships")]
    [SerializeField]
    protected int playerAttShips;
    [SerializeField]
    protected int playerDefShips;
    [SerializeField]
    protected int playerHealShips;
    [SerializeField]
    protected int playerSwarmShips;
    [SerializeField]
    protected int playerStealthShips;

    [Header("computerShips")]
    [SerializeField]
    protected int ComputerAttShips;
    [SerializeField]
    protected int ComputerDefShips;
    [SerializeField]
    protected int ComputersHealShips;
    [SerializeField]
    protected int ComputerSwarmShips;
    [SerializeField]
    protected int ComputerStealthShips;


    // Start is called before the first frame update
    protected void Awake()
    {
        p = GameObject.FindObjectOfType<Player>();
        c = GameObject.FindObjectOfType<EnemyAi>();

        tm = new Turnmanager();

        #region shippositions
        PShipPos = new Transform[5];
        CShipPos = new Transform[5];

        //find all the positions for the ships in the scene
        GameObject[] playerShipPositions = GameObject.FindGameObjectsWithTag("PlayerShipPositions");
        GameObject[] computerShipPositions = GameObject.FindGameObjectsWithTag("ComputerShipPositions");

        //get the transform.positions from these objet to load the ships from the fleet into the scene
        for (int i = 0; i < playerShipPositions.Length; i++)
        {
            PShipPos[i] = playerShipPositions[i].transform;
            CShipPos[i] = computerShipPositions[i].transform;
        }

        #endregion

    }
    

    #region ship loading


    protected void LoadPlayerShips(List<GameObject> pshipsToLoad)
    {
        for (int i = 0; i < pshipsToLoad.Count; i++)
        {
            GameObject ship = Instantiate(pshipsToLoad[i], PShipPos[i]);
            ship.name = "players " + ship.GetComponent<Ship>().GetType() + " " + i;
            ship.GetComponent<Ship>().side = 1;
            p.hangar.AddShipToFleet(ship);
        }
    }
    protected void LoadComputerShips(List<GameObject> cshipsToLoad)
    {
        for (int i = 0; i < cshipsToLoad.Count; i++)
        {
            GameObject ship = Instantiate(cshipsToLoad[i], CShipPos[i]);
            ship.name = "computers " + ship.GetComponent<Ship>().GetType() + " " + i;
            ship.GetComponent<Ship>().side = 2;
            c.hangar.AddShipToFleet(ship);
        }
    }


    #endregion

    // Update is called once per frame
    void Update()
    {

    }


}
