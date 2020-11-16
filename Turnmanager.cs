using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using System.Runtime;
using UnityEngine.Events;
using System.Diagnostics;

public class Turnmanager : MonoBehaviour
{
    private Hangar playerHangar;
    private EnemyAi computer;
 

    private Dictionary<string, GameObject> playerFleet;
    private Dictionary<string, GameObject> computerFleet;

    [SerializeField]
    private SuperScene scene;


    [SerializeField]
    private GameObject UI;

    [SerializeField]
    private DynamicButtonManager DBM;

    [SerializeField]
    private SceneManagement SCM;

    public UnityEvent onShipSelectionComplete;


    private GameObject HealthBarPrefab;

    private Canvas canvas;

    [SerializeField]
    private Text text;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        HealthBarPrefab = Resources.Load("Prefabs/HealthBar") as GameObject;


        SCM = new SceneManagement();
        scene = FindObjectOfType<SuperScene>();
        computer = FindObjectOfType<EnemyAi>();

        scene.LoadComputerShips();
        scene.LoadPlayerShips();

        this.playerHangar = PersistantDataManager.Instance.GetPlayerHangar();

        makeHealthBars();
        
        
       

        playerFleet = playerHangar.GetFleet();
        computerFleet = computer.hangar.GetFleet();

        DBM.createStateSelectionUIElement();

        for (int i = 0; i < playerFleet.Count; i++)
        {
            print("creating target buttons and state buttons for " + playerFleet.ElementAt(i).Value.name);
            DBM.createTargetButton(playerFleet.ElementAt(i).Value);
            playerFleet.ElementAt(i).Value.GetComponent<Ship>().Init();

        }

        for (int i = 0; i < computerFleet.Count; i++)
        {
            print("creating target buttons for " + computerFleet.ElementAt(i).Value.name);
            DBM.createTargetButton(computerFleet.ElementAt(i).Value);
            computerFleet.ElementAt(i).Value.GetComponent<Ship>().Init();
        }

        
        StartCoroutine(StartTurn());
        
    }

    private void Awake()
    {
        playerHangar = PersistantDataManager.Instance.GetPlayerHangar();
        computer = FindObjectOfType<EnemyAi>();
    }


    IEnumerator StartTurn()
    {
        //update fleet
        playerFleet = playerHangar.GetFleet();
        computerFleet = computer.hangar.GetFleet();

        for (int i = 0; i < playerFleet.Count; i++ )
        {
            //TODO: highlight ship for which player has to select action
            List<Ship> shiplist = playerHangar.GetShipList();
            Ship ship = shiplist[i];
            if (ship != null)
            {

                ship.state = Ship.State.neutral;


                DBM.deActivateStateButtons();
                text.text = "what would you like " + ship.name + " to do?";
                


                //add listeners to the butons to select what the ship should to this turn
                List<GameObject> shipStateButtonList = new List<GameObject>();


                shipStateButtonList = DBM.getStateButtons();

                shipStateButtonList[0].GetComponent<Button>().onClick.AddListener(delegate () { ship.setState(Ship.State.attack); });
                shipStateButtonList[1].GetComponent<Button>().onClick.AddListener(delegate () { ship.setState(Ship.State.defend); });

                if (ship is SupportClass)
                {
                    shipStateButtonList[2].GetComponent<Button>().onClick.AddListener(delegate () { ship.setState(Ship.State.heal); });
                }
                DBM.setStateSelectionPosition(ship.gameObject.transform.position);


                DBM.activateStateButtons(ship);
                //wait for state selection
                while (ship.state == Ship.State.neutral)
                {
                    yield return null;
                }
                DBM.deActivateStateButtons();
                removeListeners();

                
                text.text = string.Empty;
                yield return new WaitForSeconds(1);
                
            }
            
        }
        print("state selection for player complete");

        //let the computer choose its states for each ship
        computer.stateSelection(computer.hangar.GetShipList());

        

        //select targets now
        List<GameObject> Ships = new List<GameObject>();
        Ships.AddRange(checkAttack(playerHangar.GetFleet()));
        Ships.AddRange(checkAttack(computer.hangar.GetFleet()));
        Ships = Ships.OrderByDescending(ship => ship.GetComponent<Ship>().GetSpeed()).ToList();

        foreach (GameObject s in Ships)
        {
            Ship _ship = s.GetComponent<Ship>();
            bool isComputer = false;
            if (_ship.Side == 2)
            {
                isComputer = true;
            }
            StartCoroutine(SelectTarget(_ship, isComputer, false));
            while (_ship.GetSelectedTargets().Count < 1)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1);
        }
        print("Starting attacking phase");
        StartCoroutine(ExecuteAttacks());
    }


    IEnumerator SelectTarget(Ship _ship, bool isComputerShip, bool isReselect)
    {
        print("selecting target for" + _ship + " " + isComputerShip);
        if (!isComputerShip)
        {
            if (isReselect)
            {
                text.text = _ship.name + "'s target died reselect your target";
            }
            else if(_ship is SupportClass)
            {
                text.text = "which target would you like: " + _ship.name + " to heal?";
            }
            else
            {
                text.text = "which target would you like: " + _ship.name + " to attack?";
            }
            
        }
        computerFleet = computer.hangar.GetFleet();
        playerFleet = playerHangar.GetFleet();

        //target selection
        List <GameObject> attacking = new List<GameObject>();
        List <GameObject> aliveTargets = new List<GameObject>();
        List <GameObject> defending = new List<GameObject>();
        List <GameObject> aliveAllies = new List<GameObject>();




        if (isComputerShip)
        {
            attacking = checkAttack(playerFleet);
            defending = checkDefence(playerFleet);
            aliveTargets = playerHangar.GetFleetList();
            aliveAllies = computer.hangar.GetFleetList();
        }
        else
        {
            attacking = checkAttack(computerFleet);
            defending = checkDefence(computerFleet);
            aliveTargets = computer.hangar.GetFleetList();
            aliveAllies = playerHangar.GetFleetList();
        }

        if (_ship is SwarmClass || _ship is StealthClass)
        {
            _ship.AddTargets(aliveTargets);
        }
        else if (_ship.state == Ship.State.heal)
        {
            _ship.AddTargets(aliveAllies);
        }
        else if(!isComputerShip)
        {
            if (defending.Count >= 1)
            {
                _ship.AddTargets(defending);
            }
            else
            {
                _ship.AddTargets(attacking);
            }
        }

        //get list of possible targets for the ship to select a target for
        List<Ship> shipTargets = new List<Ship>();
        foreach (KeyValuePair<string, GameObject> element in _ship.GetTargets())
        {
            shipTargets.Add(element.Value.GetComponent<Ship>());
        }

        if (_ship is SwarmClass)
        {
            List<string> t = new List<string>();
            foreach (Ship s in shipTargets)
            {
                print("selecting all targets for swarmclass");
                t.Add(s.name);
            }
            _ship.SelectTargets(t);
        }
        else
        {

            //random computer selection
            if (isComputerShip)
            {
                computer.targetSelection(computer.hangar.GetShipList(), playerHangar.GetShipList());
            }
            //the logic for activating and deactivating the buttons for each targetable ship
            else
            {
                foreach (Ship s in shipTargets)
                {
                    //activate the button for each targetable ship
                    s.activateButton();
                    s.setButtonText(s.name);
                    s.onTargetPress.AddListener(_ship.OnTargetSelectionCallback);
                }
            }
            while (_ship.GetSelectedTargets().Count < 1)
            {
                yield return null;
            }

            foreach (Ship s in shipTargets)
            {
                //deactivate the button for each ship
                s.deactivateButton();
                s.onTargetPress.RemoveAllListeners();
            }
        }

        yield return new WaitForSeconds(1);
        text.text = string.Empty;
    }

    IEnumerator ExecuteAttacks()
    {
        //make a list with the attacking ships
        List<GameObject> plist = new List<GameObject>();
        List<GameObject> clist = new List<GameObject>();

        plist = checkAttack(playerFleet);
        clist = checkAttack(computerFleet);

        List<Ship> cShipList = new List<Ship>();
        for (int i = 0; i < clist.Count; i++)
        {
            cShipList.Add(clist[i].GetComponent<Ship>());
        }

        plist.AddRange(clist);


        //sort this list based on speed value
        List<GameObject> attackOrder = plist.OrderByDescending(ship => ship.GetComponent<Ship>().GetSpeed()).ToList();

        for (int i = 0; i < attackOrder.Count(); i++)
        {
            bool isComputerShip;
            Ship _ship = attackOrder[i].GetComponent<Ship>();

            if (_ship.Side == 1)
            {
                isComputerShip = false;
            }
            else
            {
                isComputerShip = true;
            }

            if (_ship is SwarmClass)
            {
                for (int j = 0; j < _ship.GetTargets().Count; j++)
                {
                    Ship targetShip = _ship.GetTargets().ElementAt(j).Value.GetComponent<Ship>();
                    if (targetShip.hasDied())
                    {
                        yield return new WaitForSeconds(2);
                        targetShip.DIE();
                    }
                }
            }
            else
            {
                for (int j = 0; j < _ship.GetTargets().Count; j++)
                {
                    Ship targetShip = _ship.GetTargets().ElementAt(j).Value.GetComponent<Ship>();
                    if (targetShip.hasDied())
                    {
                        targetShip.DIE();
                        _ship.clearSelectedTargets();

                        if (isComputerShip)
                        {
                            if (playerFleet.Count == 0)
                            {
                                StartCoroutine(computerWon());
                            }
                        }
                        if (!isComputerShip)
                        {
                            if (computerFleet.Count == 0)
                            {
                                StartCoroutine(playerWon());
                            }
                        }

                        print(_ship.name + "'s target died reselecting target");
                        StartCoroutine(SelectTarget(_ship, isComputerShip, true));

                        yield return new WaitForSeconds(1);
                    }
                }
            }
            _ship.attack();
            yield return new WaitForSeconds(1);




            _ship.clearTargets();
        }
        print("attacking is done checking for victory now");


        yield return new WaitForSeconds(2);


        StartCoroutine(checkOnVictory());
    }


    IEnumerator checkOnVictory()
    {
        print("Check on victory");
        if (computer.hangar.GetFleetList().Count == 0)
        {
            StartCoroutine(playerWon());
        }

        if (playerFleet.Count == 0)
        {
            StartCoroutine(computerWon());
        }

        else
        {
            StartCoroutine(StartTurn());
        }

        yield return new WaitForSeconds(2);


    }

    IEnumerator playerWon()
    {
        print("Player Won!");
        print("Player Won!");
        print("Player Won!");
        print("Player Won!");
        print("Player Won!");
        text.text = "you won!";
        PersistantDataManager.Instance.previousGameWinner = PersistantDataManager.PreviousGameWinner.player;

        yield return new WaitForSeconds(2);

        SCM.GoToScene("GalaxyMap");
    }

    IEnumerator computerWon()
    {
        print("Computer Won!");
        print("Computer Won!");
        print("Computer Won!");
        print("Computer Won!");
        print("Computer Won!");
        text.text = "you lost";
        PersistantDataManager.Instance.previousGameWinner = PersistantDataManager.PreviousGameWinner.enemyAI;

        yield return new WaitForSeconds(2);

        SCM.GoToScene("GalaxyMap");
    }
    #region attack and target selection
    List<GameObject> checkAttack(Dictionary<string, GameObject> f)
    {
        List<GameObject> list = new List<GameObject>();

        for (int i= 0; i < f.Count; i++)
        {
            Ship ship = f.ElementAt<KeyValuePair<string, GameObject>>(i).Value.GetComponent<Ship>();

            if (ship.state == Ship.State.attack || ship.state == Ship.State.heal)
            {
                list.Add(ship.gameObject);
            }
        }

        return list;
    }

    List<GameObject> checkDefence(Dictionary<string, GameObject> f)
    {
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < f.Count; i++)
        {
            Ship ship = f.ElementAt<KeyValuePair<string, GameObject>>(i).Value.GetComponent<Ship>();

            if (ship.state == Ship.State.defend)
            {
                list.Add(ship.gameObject);
            }
        }

        return list;
    }

    #endregion

    #region UI

    private void removeListeners()
    {
        List<GameObject> stateBtnList = DBM.getStateButtons();
        for (int i = 0; i < stateBtnList.Count; i++)
        {
            stateBtnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }


    //make the healthbars
    private void makeHealthBars()
    {
        List<Ship> shipList = playerHangar.GetShipList();
        shipList.AddRange(computer.hangar.GetShipList());

        for (int i = 0; i < shipList.Count; i++)
        {
            Vector3 posToLoad = shipList[i].gameObject.transform.position;
            posToLoad += new Vector3(10, 10, 0);
            
            GameObject hpBar = Instantiate(HealthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

            hpBar.transform.SetParent(canvas.transform);
            hpBar.transform.position = Camera.main.WorldToScreenPoint(posToLoad);
            if (posToLoad.y >= 265)
            {
                posToLoad -= new Vector3(0, -25, 0);
            }

            shipList[i].hpSlider = hpBar.GetComponent<Slider>();
        }

    }
    #endregion



}