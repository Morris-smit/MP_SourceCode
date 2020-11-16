using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Gamemanager : MonoBehaviour
{
    //Array met planeten met als de eerste 4 de planeten links het dichts bij de player index 5 in de centrale planeet en de laatste 4 zijn planeten het dichts bij de enemy kant
    [SerializeField]
    GameObject[] planets;

    //enemy en player line renderer
    [Header("LineRenderer")]
    [SerializeField]
    private LineRenderer EnemyLine;
    [SerializeField]
    private LineRenderer PlayerLine;
    //start locatie enemy en player (ook van de lijn)
    [SerializeField]
    private Transform playerStart;
    [SerializeField]
    private Transform EnemyStart;


    [Header("misc")]
    [SerializeField]
    private ShipGenerator shipGen;

    //dynamic buttonManager and the UI
    private DynamicButtonManager DBM;
    [SerializeField]
    private Canvas UI;

    [SerializeField]
    private GameObject fleetSelectionUIElement;

    [SerializeField]
    private GameObject battleButton;

    [SerializeField]
    private SceneManagement SCM;


    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        print("Start of GameManager");
        DBM = this.gameObject.GetComponent<DynamicButtonManager>();
        DBM.setCanvas(UI);

        //check if the player won the previous game and give him a new ship
        if (PersistantDataManager.Instance.previousGameWinner == PersistantDataManager.PreviousGameWinner.player)
        {
            string iteration = PersistantDataManager.Instance.GetPlayerHangar().GetHangar().Count.ToString();
            GameObject newShip = GameObject.Instantiate(shipGen.GenerateRandomShip(iteration));
            newShip.name = newShip.GetComponent<Ship>().GetType().ToString() + " " + iteration.ToString();

            PersistantDataManager.Instance.GetPlayerHangar().AddShipToHangar(newShip);
            newShip.SetActive(false);

            PersistantDataManager.Instance.playerGame += 1;
            PersistantDataManager.Instance.enemyGame -= 1;

            PersistantDataManager.Instance.previousGameWinner = PersistantDataManager.PreviousGameWinner.none;

            Vector3 posToLoad;
            string planetToLoad;
            posToLoad = planets[PersistantDataManager.Instance.playerGame].transform.position;
            planetToLoad = planets[PersistantDataManager.Instance.playerGame].name;
            
            posToLoad += new Vector3(0, 0, -8);
            DBM.CreateBattleButtonAtPos(posToLoad);
            DBM.battleButton.GetComponent<Button>().onClick.AddListener(delegate () { openFleetSelection(); });
            print(planets[PersistantDataManager.Instance.playerGame]);
            this.battleButton.GetComponent<Button>().onClick.AddListener(delegate () { goToScene(planetToLoad); });

            checkGameState();

            RenderLines();
        }
        else
        {

            PersistantDataManager.Instance.playerGame -= 1;
            PersistantDataManager.Instance.enemyGame += 1;

            PersistantDataManager.Instance.previousGameWinner = PersistantDataManager.PreviousGameWinner.none;

            Vector3 posToLoad;
            string planetToLoad;
            if (PersistantDataManager.Instance.playerGame <= 1)
            {
                posToLoad = planets[PersistantDataManager.Instance.playerGame].transform.position;
                planetToLoad = planets[PersistantDataManager.Instance.playerGame].name;
            }
            else
            {
                posToLoad = planets[PersistantDataManager.Instance.playerGame - 1].transform.position;
                planetToLoad = planets[PersistantDataManager.Instance.playerGame - 1].name;
            }
            posToLoad += new Vector3(0, 0, -8);
            print(planets[PersistantDataManager.Instance.playerGame].name);
            DBM.CreateBattleButtonAtPos(posToLoad);
            DBM.battleButton.GetComponent<Button>().onClick.AddListener(delegate () { openFleetSelection(); });
            this.battleButton.GetComponent<Button>().onClick.AddListener(delegate () { goToScene(planetToLoad); });

            checkGameState();

            RenderLines();
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void checkGameState()
    {
        if (PersistantDataManager.Instance.playerGame == 0)
        {
            print("the player lost");
            Application.Quit();
        }
        else
        {
            print("player is still standing");
        }
    }

    /// <summary>
    /// Renderd het eerste punt van de line en drawt de totale line
    /// </summary>
    private void RenderLines()
    {
        PlayerLine.positionCount = PersistantDataManager.Instance.playerGame + 1;
        EnemyLine.positionCount = PersistantDataManager.Instance.enemyGame;
        
        PlayerLine.SetPosition(0, playerStart.position);
        EnemyLine.SetPosition(0, EnemyStart.position);


        drawPlayerLine(PersistantDataManager.Instance.playerGame + 1);
        drawEnemyLine(PersistantDataManager.Instance.enemyGame);

        //DrawLine(game + 1);
    }

    /// <summary>
    /// renderd de lijn tot bij welke game je bent (als je bij de eerste game bent gaat de lijn tot de eerste planeet ben je bij game 2 gaat ie naar de 2e etc. je ziet de veranderingen als je realtime de game integer aanpast in game)
    /// </summary>
    /// <param name="amount">hoevaak de for loop moet loopen dus het aantal games wat is gespeeld plus de player en enemy locatie dus is altijd minimaal 1</param>
    private void DrawLine(int amount)
    {
        if (amount >= 2)
        {
            for (int i = 0; i < amount; i++)
            {
                PlayerLine.SetPosition(i, planets[i].transform.position);
                EnemyLine.SetPosition(i, planets[8 - i].transform.position);

                if (PlayerLine.GetPosition(i) == EnemyLine.GetPosition(i))
                {
                    print("Same position");
                }
            }
        }
    }

    private void drawPlayerLine(int amount)
    {
        if (amount >= 1)
        {
            for (int i = 0; i < amount; i++)
            {
                PlayerLine.SetPosition(i, planets[i].transform.position);
            }
        }
    }


    private void drawEnemyLine(int amount)
    {
        if (amount >= 1)
        {
            for (int i = 0; i < amount; i++)
            {
                EnemyLine.SetPosition(i, planets[8 - i].transform.position);
            }
        }
    }


    private void goToScene(string name)
    {
        PersistantDataManager.Instance.SetPlayerHangar(fleetSelectionUIElement.GetComponent<FleetSelectionManager>().getPlayerHangar());
        fleetSelectionUIElement.SetActive(false);
        SCM.GoToScene(name);
    }

    private void openFleetSelection()
    {
        
        fleetSelectionUIElement.GetComponent<FleetSelectionManager>().updateHangar(PersistantDataManager.Instance.GetPlayerHangar());
        Hangar hangar = PersistantDataManager.Instance.GetPlayerHangar();
        fleetSelectionUIElement.gameObject.SetActive(true);
    }
}

