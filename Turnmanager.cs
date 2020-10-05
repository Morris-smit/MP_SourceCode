using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using System.Runtime;
using System.Net.Http.Headers;

public class Turnmanager : MonoBehaviour
{
    private Player player;
    private EnemyAi computer;
 

    private Dictionary<string, GameObject> playerFleet;
    private Dictionary<string, GameObject> computerFleet;

    [SerializeField]
    private SuperScene scene;

    [SerializeField]
    private GameObject defButton;
    [SerializeField]
    private GameObject attButton;
    [SerializeField]
    private GameObject healButton;


    [SerializeField]
    private GameObject target0Button;
    [SerializeField]
    private GameObject target1Button;
    [SerializeField]
    private GameObject target2Button;

    private List<GameObject> targetButtons;



    [SerializeField]
    private Text text;

    void Start()
    {
        playerFleet = player.hangar.GetFleet();
        computerFleet = computer.hangar.GetFleet();


        StartCoroutine(StartTurn());
        
    }

    private void Awake()
    {
        //find the attack and defend buttons
        //attButton = GameObject.FindGameObjectWithTag("AttackButton");
        //defButton = GameObject.FindGameObjectWithTag("DefButton");
        targetButtons = new List<GameObject>();

        targetButtons.Add(target0Button);
        targetButtons.Add(target1Button);
        targetButtons.Add(target2Button);

        deactivateButtons();

        player = FindObjectOfType<Player>();
        computer = FindObjectOfType<EnemyAi>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator StartTurn()
    {
        //update fleet
        playerFleet = player.hangar.GetFleet();
        computerFleet = computer.hangar.GetFleet();

        for (int i = 0; i < playerFleet.Count; i++ )
        {
            //TODO: highlight ship for which player has to select action
            List<Ship> shiplist = player.hangar.GetshipList();
            Ship ship = shiplist[i];
            if (ship != null)
            {
                Ship _ship = ship.GetComponent<Ship>();
                _ship.SetState(0);
                
                // TO DO: highlight shipToHighlight
                text.text = _ship.side + " what would you like " + _ship.name + " to do?";
                
                activateButtons();
                
                //add listeners to the butons to select what the ship should to this turn
                Button ATB = attButton.GetComponent<Button>();
                ATB.onClick.AddListener(delegate () { selectStateAttack(ship.gameObject); });
                Button DB = defButton.GetComponent<Button>();
                DB.onClick.AddListener(delegate () { selectStateDefend(ship.gameObject); });

                //check if the ship object is a supportclass if so add the heal button
                if (_ship is SupportClass)
                {
                    healButton.SetActive(true);
                    Button HB = healButton.GetComponent<Button>();
                    HB.onClick.AddListener(delegate () { selectStateHeal(ship.gameObject); });
                }

                //wait for state selection
                while (_ship.GetState() == 0)
                {
                    yield return null;
                }
                ATB.onClick.RemoveAllListeners();
                DB.onClick.RemoveAllListeners();
                deactivateButtons();
                text.text = string.Empty;
                yield return new WaitForSeconds(1);
                
            }
            
        }
        print("state selection for player complete");
        //to do implement minimax
        for (int i = 0; i < computerFleet.Count; i++ )
        {
            //GameObject ship = computerFleet.ElementAt<KeyValuePair<string, GameObject>>(i).Value;
            List<Ship> ships = computer.hangar.GetshipList();

            Ship ship = ships[i];



            int state = Random.Range(0, 10);
            if (state <= 5)
            {
                selectStateDefend(ship.gameObject);
                
            }
            else
            {
                selectStateAttack(ship.gameObject);
            }            
        }

        print("state selection for computer complete");

        //do attacks now
        StartCoroutine(SelectTarget()); 
    }


    IEnumerator SelectTarget()
    {
        //player target selection
        List <GameObject> attacking = new List<GameObject>();
        List <GameObject> defending = new List<GameObject>();

        for (int i = 0; i < playerFleet.Count; i++)
        {
            //var ship = playerFleet.ElementAt(i).Value;
            List<Ship> pfleet = player.hangar.GetshipList(); 
            Ship ship = pfleet[i];
            Ship _ship = ship.GetComponent<Ship>();
            _ship.AddTargets(computer.hangar.GetFleetList());


            //check if the ship to select a target for is the swarm class, if so skip to the next player ship
            if (_ship is SwarmClass)
            {
                List<string> t = new List<string>();
                foreach (Ship s in computer.hangar.GetshipList())
                {
                    t.Add(s.name);
                }
                _ship.SelectTargets(t);

                i++;
                ship = pfleet[i];
            }

            

            attacking = checkAttack(computerFleet);
            defending = checkDefence(computerFleet);

            

            if (_ship.GetState() != 2)
            {
                if (_ship is StealthClass)
                {
                    attacking = computer.hangar.GetFleetList();
                }
                else if (_ship.GetState() == 3)
                {
                    attacking = player.hangar.GetFleetList();
                    computerFleet = player.hangar.GetFleet();
                }

                //if there are no enemy defending ships
                if (attacking.Count == computerFleet.Count)
                {
                    

                    int count = 0;
                    //create buttons for each targetable ship
                    foreach (KeyValuePair<string, GameObject> element in computerFleet)
                    {
                        var _targetShip = computerFleet.ElementAt(count).Value;
                        List<string> targets = new List<string>();
                        targets.Add(_targetShip.name);
                        Ship _tship = _targetShip.GetComponent<Ship>();

                        print("selecting target for: " + _ship.name + " " + _ship.side);
                        text.text = "which ship would you like " + _ship.name + " to attack?";

                        Button button = targetButtons[count].GetComponent<Button>();


                        //activate the button
                        button.gameObject.SetActive(true);


                        //add a listener to this button and a targetable ship

                        //targetShip = computerShips.ElementAt<KeyValuePair<string, GameObject>>(1).Value;
                        button.onClick.AddListener(delegate () { selectTarget(ship.gameObject, targets); });

                        Text _text = button.gameObject.GetComponentInChildren<Text>();
                        _text.text = _targetShip.name;
                        

                        count++;
                    }
                    {
                        yield return null;
                    }
                    attButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    defButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    healButton.GetComponent<Button>().onClick.RemoveAllListeners();

                    deactivateButtons();
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    //to do: make logic for defending ships and for stealth ships to skip this logic

                    //make target buttons for each targetable ship
                    for (int j = 0; j < defending.Count; j++)
                    {
                        GameObject _targetShip = defending[j];
                        List<string> targets = new List<string>();
                        targets.Add(_targetShip.name);
                        Ship _tship = _targetShip.GetComponent<Ship>();


                        print("selecting target for: " + _ship.name + " " + _ship.side);
                        text.text = "which ship would you like " + _ship.name + " to attack?";

                        Button button = targetButtons[j].GetComponent<Button>();

                        //activate the button
                        button.gameObject.SetActive(true);

                        //add a listener to this button and a targetable ship

                        button.onClick.AddListener(delegate () { selectTarget(_ship.gameObject, targets); });


                        Text _text = button.gameObject.GetComponentInChildren<Text>();
                        _text.text = _targetShip.name;

                        
                    }
                    while (_ship.GetSelectedTargets().Count < 1)
                    {
                        yield return null;
                    }
                    attButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    defButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    healButton.GetComponent<Button>().onClick.RemoveAllListeners();

                    deactivateButtons();
                    yield return new WaitForSeconds(1);
                }
                
            }
        }
        
        //to do implement minimax

        //make the computer select a target
        attacking = checkAttack(playerFleet);
        defending = checkDefence(playerFleet);


        //if there are no player ships on defence select a target from the list of attacking player ships
        if (defending.Count <= 0)
        {
            

            for (int i = 0; i < attacking.Count; i++)
            {

                var ship = computerFleet.ElementAt(i).Value;
                Ship _ship = ship.GetComponent<Ship>();
                _ship.AddTargets(player.hangar.GetFleetList());

                //check if the ship to select a target for is a swarm class ship
                if (_ship is SwarmClass)
                {
                    List<string> t = new List<string>();
                    foreach (Ship s in player.hangar.GetshipList())
                    {
                        t.Add(s.name);
                    }
                    _ship.SelectTargets(t);
                    i++;
                }

                ship = computerFleet.ElementAt(i).Value;
                _ship = ship.GetComponent<Ship>();


                if (_ship.GetState() != 2)
                {
                    //select a random target from the list of attacking ships?
                    List<string> t = new List<string>();
                    t.Add(attacking[Random.Range(0, attacking.Count)].name);
                    selectTarget(ship, t);
                }
            }

            print("target selection complete");
            yield return new WaitForSeconds(2);
        }
        //if there are player ships on defence select a random target from the list of defending player ships
        else
        {
            for (int i = 0; i < defending.Count; i++)
            {

                var ship = defending[i];
                Ship _ship = ship.GetComponent<Ship>();

                //check if the ship to select a target for is a swarm class ship
                if (_ship is SwarmClass)
                {
                    List<string> t = new List<string>();
                    foreach (Ship s in player.hangar.GetshipList())
                    {
                        t.Add(s.name);
                    }
                    _ship.SelectTargets(t);
                    i++;
                }

                 


                if (_ship.GetState() != 2)
                {
                    //select a random target from the list of attacking ships?
                    List<string> t = new List<string>();
                    t.Add(attacking[Random.Range(0, attacking.Count)].name);
                    selectTarget(ship, t);
                }
            }

            print("target selection complete");
            yield return new WaitForSeconds(2);
        }
        
        StartCoroutine(ExecuteAttacks());
    }

    IEnumerator ExecuteAttacks()
    {
        bool attacking = true;

        //make a list with the attacking ships
        List<GameObject> plist = new List<GameObject>();
        List<GameObject> clist = new List<GameObject>();

        plist = checkAttack(playerFleet);
        clist = checkAttack(computerFleet);

        plist.AddRange(clist);


        //sort this list based on speed value
        IEnumerable<GameObject> attackOrder = plist.OrderBy(ship => ship.GetComponent<Ship>().GetSpeed());

        int count = 0;
        foreach(GameObject ship in attackOrder)
        {
            Ship _ship = ship.GetComponent<Ship>();
            _ship.attack();

            if (!_ship.HasAttacked())
            {
                StartCoroutine(reselectTarget(ship));
            }

            if (_ship is SwarmClass)
            {
                for (int i = 0; i < _ship.GetTargets().Count; i++)
                {
                    Ship targetShip = _ship.GetTargets().ElementAt(i).Value.GetComponent<Ship>();
                    if (targetShip.hasDied())
                    {
                        player.hangar.removeShipfromFleet(targetShip.gameObject);
                    }
                }
            }

            _ship.clearTargets();

            count++;
        }


        while (attacking)
        {
            yield return null;
        }
        //to do set attacking to false
    }

    IEnumerator reselectTarget(GameObject ship)
    {
        List<GameObject> attacking = new List<GameObject>(0);
        List<GameObject> defending = new List<GameObject>(0);

        Ship _ship = ship.GetComponent<Ship>();
        if (_ship.side == 1)
        {
            attacking = checkAttack(computerFleet);
            defending = checkDefence(computerFleet);
        }
        else if(_ship.side == 2)
        {
            attacking = checkAttack(playerFleet);
            defending = checkDefence(playerFleet);
        }
        

        //if the ship is on the player side
        if (_ship.side == 1)
        {
            if (attacking.Count <= 0)
            {
                int count = 0;
                //create buttons for each targetable ship
                foreach (KeyValuePair<string, GameObject> element in computerFleet)
                {
                    GameObject _targetShip = defending[count];
                    List<string> targets = new List<string>();
                    targets.Add(_targetShip.name);
                    Ship _tship = _targetShip.GetComponent<Ship>();

                    Button button = targetButtons[count].GetComponent<Button>();



                    //activate the button
                    button.gameObject.SetActive(true);


                    //add a listener to this button and a targetable ship
                    button.onClick.AddListener(delegate () { selectTarget(ship, targets); });


                    count++;
                }
                while (_ship.GetTargets().Count < 1)
                {
                    yield return null;
                }

                deactivateButtons();
                yield return new WaitForSeconds(1);
            }
            else
            {
                //to do: make logic for defending ships and for stealth ships to skip this logic

                //make target buttons for each targetable ship
                for (int j = 0; j < defending.Count; j++)
                {
                    GameObject _targetShip = defending[j];
                    List<string> targets = new List<string>();
                    targets.Add(_targetShip.name);
                    Ship _tship = _targetShip.GetComponent<Ship>();

                    Button button = targetButtons[j].GetComponent<Button>();

                    //activate the button
                    button.gameObject.SetActive(true);

                    //add a listener to this button and a targetable ship

                    button.onClick.AddListener(delegate () { selectTarget(ship, targets); });


                    //Text _text = button.GetComponent<Text>();
                    //_text.text = ship.name;


                }
                while (_ship.GetTargets().Count < 1)
                {
                    yield return null;
                }
                deactivateButtons();
                yield return new WaitForSeconds(1);

            }
        }
        //if the ship is of the enemy side
        else
        {
            defending = checkDefence(playerFleet);
            attacking = checkAttack(playerFleet);
            if (defending.Count <= 0)
            {
                for (int i = 0; i < computerFleet.Count; i++)
                {
                    ship = computerFleet.ElementAt(i).Value;
                    _ship = ship.GetComponent<Ship>();


                    if (_ship.GetState() != 2)
                    {
                        //select a random target from the list of attacking ships?
                        List<string> t = new List<string>();
                        t.Add(attacking[Random.Range(0, attacking.Count)].name);
                        selectTarget(ship, t);
                    }
                }

                print("target selection complete");
                yield return new WaitForSeconds(2);
            }
            //if there are player ships on defence select a random target from the list of defending player ships
            else
            {
                for (int i = 0; i <= defending.Count; i++)
                {
                    ship = computerFleet.ElementAt(i).Value;
                    _ship = ship.GetComponent<Ship>();


                    if (_ship.GetState() != 2)
                    {
                        //select a random target from the list of attacking ships?
                        List<string> t = new List<string>();
                        t.Add(attacking[Random.Range(0, defending.Count)].name);
                        selectTarget(ship, t);
                    }
                }

                print("target selection complete");
                yield return new WaitForSeconds(2);
            }
        }
    }

    #region attack and target selection
    void selectStateAttack(GameObject s)
    {
        if (s == null)
        {
            print("no ship found");
        }
        else
        {
            Ship _s = s.GetComponent<Ship>();
            _s.SetState(1);
            deactivateButtons();
            print(s.gameObject.name + " " + _s.side + " is on attack");
        }
        
    }

    void selectStateDefend(GameObject s)
    {
        if (s == null)
        {
            print("no ship found");
        }
        else
        {
            Ship _s = s.GetComponent<Ship>();
            _s.SetState(2);
            deactivateButtons();
            print(s.name + " " + _s.side + " is on defence");
        }
    }

    void selectStateHeal(GameObject s)
    {
        if (s == null)
        {
            print("idk what this is");
        }
        else
        {
            Ship _s = s.GetComponent<Ship>();
            _s.SetState(3);
            deactivateButtons();
        }
    }

    void selectTarget(GameObject s, List<string> t)
    {
        Ship ship = s.GetComponent<Ship>(); 

        ship.SelectTargets(t);
        print(ship.name + "'s target set to: " + ship.GetSelectedTargets()[0]);
    }

    List<GameObject> checkAttack(Dictionary<string, GameObject> f)
    {
        List<GameObject> list = new List<GameObject>();

        for (int i= 0; i < f.Count; i++)
        {
            Ship ship = f.ElementAt<KeyValuePair<string, GameObject>>(i).Value.GetComponent<Ship>();

            if (ship.GetState() == 1)
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

            if (ship.GetState() == 2)
            {
                list.Add(ship.gameObject);
            }
        }

        return list;
    }

    #endregion

    #region buttons
    //activate buttons
    protected void activateButtons()
    {
        attButton.SetActive(true);
        defButton.SetActive(true);
        
    }
    //deactivate buttons
    protected void deactivateButtons()
    {
        text.text = string.Empty;
        attButton.SetActive(false);
        defButton.SetActive(false);
        healButton.SetActive(false);

        for (int i = 0; i < targetButtons.Count; i++)
        {
            GameObject _button = targetButtons[i];
            _button.SetActive(false);
        }
    }
    #endregion



}


