using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAi : Unit
{
    List<Ship> attackShips = new List<Ship>();
    List<Ship> tankShips = new List<Ship>();
    List<Ship> supportShips = new List<Ship>();
    List<Ship> stealthShips = new List<Ship>();
    List<Ship> swarmships = new List<Ship>();

    List<Ship> aShips = new List<Ship>();
    List<Ship> attEnemyShips = new List<Ship>();
    List<Ship> defEnemyShips = new List<Ship>();
    List<Ship> healingEnemyShips = new List<Ship>();

    List<Ship> EnemySpeedList = new List<Ship>();
    List<Ship> EnemyDamageList = new List<Ship>();


    List<GameObject> possibleTargets = new List<GameObject>();

    public enum Strategy
    {
        offensive,
        defensive,
        supportive
    }

    public Strategy strategy;



    // Start is called before the first frame update
    void Start()
    {
       
    }

    new void Awake()
    {
        hangar = this.GetComponent<Hangar>();
    }

    public void stateSelection(List<Ship> alliedShips)
    {
        aShips = alliedShips;


        //get the types of the allied ships
        for (int j = 0; j < alliedShips.Count; j++)
        {
            Ship ship = alliedShips[j];
            if (ship is SwarmClass)
            {
                swarmships.Add(ship);
            }
            else if (ship is TankClass)
            {
                tankShips.Add(ship);
            }
            else if (ship is AttackClass)
            {
                attackShips.Add(ship);
            }
            else if (ship is SupportClass)
            {
                supportShips.Add(ship);
            }
            else
            {
                stealthShips.Add(ship);
            }
        }
        if (this.strategy == EnemyAi.Strategy.offensive)
        {
            offensiveStateSelection();
        }

        print("state selection for computer complete");
    }
    public void targetSelection(List<Ship> alliedShips, List<Ship> enemyShips)
    {
        aShips = alliedShips;
        List<Ship> attackingShips = checkAttack(alliedShips);
        attEnemyShips = checkAttack(enemyShips);
        defEnemyShips = checkDefence(enemyShips);
        healingEnemyShips = checkHeal(enemyShips);

        EnemySpeedList = GetListBasedOnSpeed(enemyShips);
        EnemyDamageList = GetListBasedOnDamage(enemyShips);

        //get all the possible target ships
        List<GameObject> possibleTargets = new List<GameObject>();
        for (int i = 0; i < enemyShips.Count; i++)
        {
            possibleTargets.Add(enemyShips[i].gameObject);
        }
        for (int j = 0; j < attackingShips.Count; j++)
        {
            attackingShips[j].AddTargets(possibleTargets);
        }


        for (int j = 0; j < alliedShips.Count; j++)
        {
            Ship ship = alliedShips[j];
            if (ship is SwarmClass)
            {
                swarmships.Add(ship);
            }
            else if (ship is TankClass)
            {
                tankShips.Add(ship);
            }
            else if (ship is AttackClass)
            {
                attackShips.Add(ship);
            }
            else if (ship is SupportClass)
            {
                supportShips.Add(ship);
            }
            else
            {
                stealthShips.Add(ship);
            }
        }

        if (this.strategy == EnemyAi.Strategy.offensive)
        {
            offensiveTargetSelection();
        }
    }
    #region offensive
    public void offensiveStateSelection()
    {
        //if there are atleast 1 tankship alive and 1 attackship, make sure the tank ship goes on defence
        if (tankShips.Count == 1 && attackShips.Count >= 1)
        {
            tankShips[0].state = Ship.State.defend;
            for (int i = 0; i < attackShips.Count; i++)
            {
                attackShips[i].state = Ship.State.attack;
            }
        }
        //if there are only attack ships left make sure they both attack
        if (tankShips.Count == 0 && attackShips.Count == 2)
        {
            attackShips[0].state = Ship.State.attack;
            attackShips[1].state = Ship.State.attack;
        }
        if (tankShips.Count == 0 && attackShips.Count == 1)
        {
            attackShips[0].state = Ship.State.attack;
        }
        //if the tank ship is the last ship make sure it attacks
        if (tankShips.Count == 1 && attackShips.Count == 0)
        {
            tankShips[0].state = Ship.State.attack;
        }
        clearShips();
    }
    public void offensiveTargetSelection()
    {
        List<Ship> attackingShips = checkAttack(aShips);

        

        List<Ship> SpeedList = GetListBasedOnSpeed(aShips);
        List<Ship> DamageList = GetListBasedOnDamage(aShips);


        if (defEnemyShips.Count > 0)
        {
            if (defEnemyShips.Count > 1)
            {   
                if (attackShips.Count > 1)   
                {
                    //check if the AI has enoug attacks to destroy both defending ships
                    if(
                        attackingShips[0].GetAttDamage() >= defEnemyShips[0].GetHealth() &&
                        attackingShips[1].GetAttDamage() >= defEnemyShips[1].GetHealth()
                    ){
                        attackingShips[0].SelectTargets(defEnemyShips[0].name);
                        attackingShips[1].SelectTargets(defEnemyShips[1].name);
                    }
                    if(
                        attackingShips[1].GetAttDamage() >= defEnemyShips[0].GetHealth() &&
                        attackingShips[0].GetAttDamage() >= defEnemyShips[1].GetHealth()
                    ){
                        attackingShips[1].SelectTargets(defEnemyShips[0].name);
                        attackingShips[0].SelectTargets(defEnemyShips[1].name);
                    }
                    //check if the ai can destroy atleast 1 ship and damage the other
                    if(
                        attackingShips[1].GetAttDamage() + attackingShips[0].GetAttDamage() >= defEnemyShips[0].GetHealth()
                    ){
                        attackingShips[0].SelectTargets(defEnemyShips[0].name);
                        attackingShips[1].SelectTargets(defEnemyShips[0].name);
                    }
                    else
                    {
                        attackingShips[0].SelectTargets(defEnemyShips[1].name);
                        attackingShips[1].SelectTargets(defEnemyShips[1].name);
                    }
                }
                //if there is 1 attacking ship left check if it can destroy a tank ship
                else
                {
                    if (attackingShips[0].GetAttDamage() >= defEnemyShips[0].GetHealth())
                    {
                        attackingShips[0].SelectTargets(defEnemyShips[0].name);
                    }
                    if (attackingShips[0].GetAttDamage() >= defEnemyShips[1].GetHealth())
                    {
                        attackingShips[0].SelectTargets(defEnemyShips[1].name);
                    }
                    //if the attacking ship cant destroy one of the defending ships 
                    //find the defending ship with the lowest health and select that as a target for the last attacking ship
                    else
                    {
                        
                        Ship shipToAttack;
                        if (defEnemyShips[0].GetHealth() > defEnemyShips[1].GetHealth())
                        {
                            shipToAttack = defEnemyShips[0];
                        }
                        else
                        {
                            shipToAttack = defEnemyShips[1];
                        }
                        attackingShips[0].SelectTargets(shipToAttack.name);
                    }
                }
            }
            //if there is one defending enemy ship
            if (defEnemyShips.Count == 1)
            {
                //if there is more than 1 allied ship on attack
                //check if AI has enough damage to destroy the last attack ship and have an attack left
                if (attackingShips.Count > 1)
                {
                    if (healingEnemyShips.Count >= 1)
                    {
                        //make sure the AI checks if there is a healing ship present, if so account for its healing amoutn
                        //and check if the AI still has enough damage to kill the defending ship
                        Ship _HighestEnemyDamageShip = EnemyDamageList[0];
                        int enemyDefenceHealth = defEnemyShips[0].GetHealth() + healingEnemyShips[0].GetAttDamage();
                        if (attackingShips[0].GetAttDamage() >= enemyDefenceHealth)
                        {
                            attackingShips[0].SelectTargets(defEnemyShips[0].name);
                            attackingShips[1].SelectTargets(_HighestEnemyDamageShip.name);
                        }

                        if (attackingShips[1].GetAttDamage() >= enemyDefenceHealth)
                        {
                            attackingShips[1].SelectTargets(defEnemyShips[0].name);
                            attackingShips[0].SelectTargets(_HighestEnemyDamageShip.name);
                        }

                        if (attackingShips[0].GetAttDamage() + attackingShips[1].GetAttDamage() >= enemyDefenceHealth)
                        {
                            attackingShips[0].SelectTargets(defEnemyShips[0].name);
                            attackingShips[1].SelectTargets(defEnemyShips[0].name);
                        }
                    }
                    Ship HighestEnemyDamageShip = EnemyDamageList[0];
                    //check if the AI has enough attacks to destroy the remaining defending ship
                    //and attack the remaining enemy attacking ship
                    if (attackingShips[0].GetAttDamage() >= defEnemyShips[0].GetHealth())
                    {
                        attackingShips[0].SelectTargets(defEnemyShips[0].name);
                        attackingShips[1].SelectTargets(HighestEnemyDamageShip.name);
                    }
                    if (attackingShips[1].GetAttDamage() >= defEnemyShips[0].GetHealth())
                    {
                        attackingShips[1].SelectTargets(defEnemyShips[0].name);
                        attackingShips[0].SelectTargets(HighestEnemyDamageShip.name);
                    }
                    //if the AI does not have enough attacks to do either of these then destroy the defending ship
                    else
                    {
                        attackingShips[0].SelectTargets(defEnemyShips[0].name);
                        attackingShips[1].SelectTargets(defEnemyShips[0].name);
                    }
                }
                //if the Ai has 1 remaining attacking ship
                else
                {
                    attackingShips[0].SelectTargets(defEnemyShips[0].name);
                }
            }
        }
        //if there are no enemy defending ships
        else
        {
            //make sure the Ai ship with the highest speed attacks the enemy ship with the highest damage
            for (int i = 0; i < attackingShips.Count; i++)
            {
                Ship shipToSelectFor = SpeedList[i];
                if (i > EnemyDamageList.Count - 1)
                {
                    shipToSelectFor.SelectTargets(EnemyDamageList[0].name);
                }
                else
                {
                    Ship shipToAttack = EnemyDamageList[i];
                    shipToSelectFor.SelectTargets(shipToAttack.name);
                    print(shipToSelectFor.name + " selected" + shipToAttack.name + " as target");
                }
            }
        }


    }
    #endregion
    #region supportive
    public void supportiveStateSelection()
    {
        //TODO: check the ships that are alive and set their states
    }
    #endregion
    #region defensive
    public void defensiveStateSelection()
    {
        //TODO: check the ships that are alive and set their states
    }
    #endregion
    private void clearShips()
    {
        swarmships.Clear();
        tankShips.Clear();
        attackShips.Clear();
        supportShips.Clear();
        stealthShips.Clear();
    }

    List<Ship> checkAttack(List<Ship> s)
    {
        List<Ship> list = new List<Ship>();

        for (int i = 0; i < s.Count; i++)
        {
            if (s[i].state == Ship.State.attack)
            {
                list.Add(s[i]);
            }
        }

        return list;
    }
    List<Ship> checkDefence(List<Ship> s)
    {
        List<Ship> list = new List<Ship>();

        for (int i = 0; i < s.Count; i++)
        {
            if (s[i].state == Ship.State.defend)
            {
                list.Add(s[i]);
            }
        }

        return list;
    }
    List<Ship> checkHeal(List<Ship> s)
    {
        List<Ship> list = new List<Ship>();

        for (int i = 0; i < s.Count; i++)
        {
            if (s[i].state == Ship.State.heal)
            {
                list.Add(s[i]);
            }
        }

        return list;
    }

    private List<Ship> GetListBasedOnSpeed(List<Ship> list)
    {
        list.OrderBy(ship => ship.GetSpeed());
        return list;
    }
    private List<Ship> GetListBasedOnDamage(List<Ship> list)
    {
        list.OrderByDescending(ship => ship.GetHealth());
        return list;
    }
}
