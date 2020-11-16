using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEngine;

public class SuperScene : MonoBehaviour
{
    protected Turnmanager tm;

    protected Hangar PlayerHangar;

    [SerializeField]
    protected EnemyAi enemyAI;








    protected List<GameObject> pShipsToLoad;
    [SerializeField]
    protected List<GameObject> cShipsToLoad;

    protected Transform[] PShipPos;
    protected Transform[] CShipPos;

    [SerializeField]
    private EnemyAi.Strategy strategy;



    protected void Awake()
    {        
        enemyAI = GameObject.FindObjectOfType<EnemyAi>();

        tm = Object.FindObjectOfType<Turnmanager>();
        enemyAI.strategy = this.strategy;
        #region shippositions
        PShipPos = new Transform[5];
        CShipPos = new Transform[5];

        //find all the positions for the ships in the scene
        GameObject[] playerShipPositions = GameObject.FindGameObjectsWithTag("PlayerShipPositions");
        GameObject[] computerShipPositions = GameObject.FindGameObjectsWithTag("ComputerShipPositions");

        //get the transform.positions from these gameobjects to load the ships from the fleet into the scene
        for (int i = 0; i < playerShipPositions.Length; i++)
        {
            PShipPos[i] = playerShipPositions[i].transform;
            CShipPos[i] = computerShipPositions[i].transform;
        }


        #endregion

        for (int i = 0; i < cShipsToLoad.Count; )
        {
            GameObject shipToAdd = GameObject.Instantiate(cShipsToLoad[i]);
            shipToAdd.name = "computers " + shipToAdd.GetComponent<Ship>().GetType().ToString() + " " + i.ToString();
            enemyAI.hangar.AddShipToFleet(shipToAdd);
            shipToAdd.SetActive(false);
            i++;
        }
    }
    

    #region ship loading

    public void LoadPlayerShips()
    {
        Hangar _hangar = PersistantDataManager.Instance.GetPlayerHangar();
        pShipsToLoad = _hangar.GetFleetList();
        if (_hangar.GetShipList() == null)
        {
            print("ship values in fleet are null");
        }
        for (int i = 0; i < pShipsToLoad.Count; i++)
        {
            Debug.Log("Ship to load is: " + pShipsToLoad[i].name);
            pShipsToLoad[i].SetActive(true);
            pShipsToLoad[i].gameObject.transform.position = PShipPos[i].position;
            pShipsToLoad[i].gameObject.transform.rotation = PShipPos[i].rotation;
            GameObject ship = pShipsToLoad[i];
            ship.GetComponent<Ship>().Side = 1;
        }
    }
    public void LoadComputerShips()
    {
        List<GameObject> cShipList = enemyAI.hangar.GetFleetList();
        for (int i = 0; i < cShipList.Count; i++)
        {
            Debug.Log("Ship to load is: " + cShipList[i].name);
            cShipList[i].SetActive(true);
            cShipList[i].gameObject.transform.position = CShipPos[i].position;
            cShipList[i].gameObject.transform.rotation = CShipPos[i].rotation;

            GameObject ship = cShipList[i];
            ship.GetComponent<Ship>().Side = 2;   
        }
    }


    #endregion
}
