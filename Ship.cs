using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private List<string> selectedTargets;
    protected Dictionary<string, GameObject> Targets;

    //1 is for attack and 2 is for defend and 3 is for heal
    protected int state;

    //1 is for player ship 2 is for enemy ship
    public int side;

    private Turnmanager tm;

    [SerializeField]
    protected float attackdamage;
    [SerializeField]
    protected int speed;

    public Hangar hangar;

    [SerializeField]
    protected float health;

    protected bool hasAttacked;


    void Start()
    {
        this.tm = FindObjectOfType<Turnmanager>();
        this.state = 0;
    }

    void Update()
    {
        
    }

    public void SetState(int s)
    {
        this.state = s;
    }

    public int GetState()
    {
        return this.state;
    }


    public int GetSpeed()
    {
        return this.speed;
    }

    string getName()
    {
        return name;
    }

    public void takeDamage(float amount)
    {
        if (this.health - amount <= 0)
        {
            print(this.name + " was attacked and got destroyed");
            this.Die();
        }
        else
        {
            this.health -= amount;
            print(this.name + " was attacked and got damaged, remaining health: " + this.health);
        }
    }

    public virtual void attack()
    {
        foreach(string t in selectedTargets)
        {
            Ship tship = Targets[t].GetComponent<Ship>();

            if (!tship.hasDied())
            {
                print(this.name + " Attacked " + Targets[t]);
                tship.takeDamage(this.attackdamage);
                hasAttacked = true;
            }
            else
            {
                hasAttacked = false;
            }
        }
        
    }

    public void Die()
    {
        hangar.removeShipfromFleet(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void AddTargets(List<GameObject> s) 
    {
        if (Targets == null)
        {
            Targets = new Dictionary<string, GameObject>(0);
        }


        Targets.Clear();

        foreach (GameObject ship in s)
        {
            Targets.Add(ship.name, ship);
        }

        
    }

    public void SelectTargets(List<string> t)
    {
        if (selectedTargets == null)
        {
            selectedTargets = new List<string>();
        }
        selectedTargets = t;
    }

    public virtual void SetTarget(Dictionary<string, GameObject> targets)
    {
        Targets = targets;
    }

    public void clearTargets()
    {
        Targets.Clear();
    }

    public Dictionary<string, GameObject> GetTargets()
    {
        if (Targets == null)
        {
            Targets = new Dictionary<string, GameObject>(0);
        }

        return Targets;
    }

    public List<string> GetSelectedTargets()
    {
        if (selectedTargets == null)
        {
            selectedTargets = new List<string>();
        }
        return selectedTargets;
    }

    public bool hasDied()
    {
        if (this.health <= 0)
        {
            Debug.Log(this + " died");
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool HasAttacked()
    {
        return hasAttacked;
    }

    public void SetHangar(Hangar h)
    {
        this.hangar = h;
    }
}
