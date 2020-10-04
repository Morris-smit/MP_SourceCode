using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private GameObject Target;
    public Dictionary<string, GameObject> Targets;

    //1 is for attack and 2 is for defend and 3 is for heal
    private int state;

    //1 is for player ship 2 is for enemy ship
    public int side;

    private Turnmanager tm;

    [SerializeField]
    protected float attackdamage;
    [SerializeField]
    protected int speed;

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
    public void Attack()
    {
        print(this.name + " Attacked " + Target + "!");
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
            //this.die();
        }
        else
        {
            print(this.name + " was attacked and got damaged, remaining health: " + this.health);
            this.health -= amount;
        }
    }

    public virtual void attack()
    {
        Ship tship = Target.GetComponent<Ship>();

        if (!tship.hasDied())
        {
            print(this.name + " Attacked " + Target);
            tship.takeDamage(this.attackdamage);
            hasAttacked = true;
        }
        else
        {
            hasAttacked = false;
        }  
    }

    public virtual void SetTarget(Ship s) 
    {
        this.Target = s.gameObject;
    }

    public virtual void SetTarget(Dictionary<string, GameObject> targets)
    {
        Targets = targets;
    }

    public void clearTargets()
    {
        Targets.Clear();
    }
    public Dictionary<string, GameObject> _GetTarget()
    {
        return this.Targets;
    }
    public GameObject GetTarget()
    {
        return Target;
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
}
