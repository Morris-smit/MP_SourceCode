using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantDataManager : MonoBehaviour
{
    public static PersistantDataManager Instance;

    public int playerGame;
    public int enemyGame;

    public static Player player;
    public static GameObject playerGo;

    [SerializeField]
    private GameObject playerPrefab;

    public enum PreviousGameWinner
    {
        player,
        enemyAI,
        neutralAI,
        none
    }

    public PreviousGameWinner previousGameWinner;


    [SerializeField]
    private List<GameObject> defaultShips;


    private void Awake()
    {
        if (Instance == null)
        {
            playerPrefab = Resources.Load("prefabs/Player") as GameObject;

            Instance = this;
            playerGo = GameObject.Instantiate(playerPrefab);
            player = playerGo.GetComponent<Player>();

            DontDestroyOnLoad(player);
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
            //Destroy(player);
        }

        if (playerGo.GetComponent<Player>().hangar.GetHangar().Count == 0)
        {
            setDefaultShips();
        }
    }

    private void setDefaultShips()
    {
        for (int i = 0; i < defaultShips.Count; i++)
        {
            GameObject ShipOBj = Instantiate(defaultShips[i]);
            ShipOBj.SetActive(false);
            ShipOBj.name = ShipOBj.GetComponent<Ship>().GetType().ToString() + " " + i.ToString();
            Ship ship = ShipOBj.GetComponent<Ship>();
            ship.name = ship.gameObject.name;
            ship.InitializeStats();
            //ship.hangar = GetPlayerHangar();
            player.hangar.AddShipToHangar(ShipOBj);
        }
    }

    public Hangar GetPlayerHangar()
    {
        Hangar _hangar = player.GetComponent<Hangar>();
        return player.GetComponent<Hangar>();
    }

    public void SetPlayerHangar(Hangar hangar)
    {
        player.GetComponent<Player>().Sethangar(hangar);
        Hangar _hangar = player.GetComponent<Hangar>();
    }
}
