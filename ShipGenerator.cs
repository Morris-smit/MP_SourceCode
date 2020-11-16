using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator : MonoBehaviour
{
    private GameObject attackShipPrefab;
    private GameObject tankShipPrefab;
    private GameObject stealthShipPrefab;
    private GameObject supportShipPrefab;
    private GameObject swarmShipPrefab;

    private List<GameObject> prefabList;

    private void Awake()
    {
        prefabList = new List<GameObject>();

        prefabList.Add(attackShipPrefab = Resources.Load("Prefabs/Pships/PattShip") as GameObject);
        prefabList.Add(tankShipPrefab = Resources.Load("Prefabs/Pships/PtankShip") as GameObject);
        prefabList.Add(stealthShipPrefab = Resources.Load("Prefabs/Pships/PstealthShip") as GameObject);
        prefabList.Add(supportShipPrefab = Resources.Load("Prefabs/Pships/PhealShip") as GameObject);
        prefabList.Add(swarmShipPrefab = Resources.Load("Prefabs/Pships/PSwarmShip") as GameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GenerateRandomShip(string iteration)
    {

        GameObject shipToLoad = prefabList[Random.Range(0, 5)];
        GameObject shipGO = shipToLoad;

        shipGO.GetComponent<Ship>().InitializeStats();

        shipGO.GetComponent<Ship>().increaseSpeed(Random.Range(0, 2));
        shipGO.GetComponent<Ship>().increaseHealth(Random.Range(0, 2));
        shipGO.GetComponent<Ship>().increaseDamage(Random.Range(0, 2));
        if (shipGO.GetComponent<Ship>() is SupportClass)
        {
            shipGO.GetComponent<Ship>().increaseHealAmount(Random.Range(0, 2));
        }

        shipGO.name = shipGO.GetComponent<Ship>().ToString() + " " + iteration.ToString();
        shipGO.GetComponent<Ship>().name = shipGO.name;

        shipGO.SetActive(false);
        return shipGO;
    }
}
