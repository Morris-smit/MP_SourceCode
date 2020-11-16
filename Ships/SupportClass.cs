using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportClass : Ship
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void InitializeStats()
    {
        this.health = 7;
        this.attackdamage = 4;
        this.speed = 7;
        this.healAmount = 8;
    }
    public override void attack()
    {
        if (this.state == Ship.State.attack)
        {
            foreach (string t in selectedTargets)
            {
                Ship tSHip = PossibleTargets[t].GetComponent<Ship>();

                if (!tSHip.hasDied())
                {
                    print(this.name + " attacked " + PossibleTargets[t]);
                    tSHip.takeDamage(this.attackdamage);
                }
            }
        }
        else
        {
            healTarget();
        }
        
    }

    public void healTarget()
    {
        foreach (string t in selectedTargets)
        {
            Ship tSHip = PossibleTargets[t].GetComponent<Ship>();

            if (!tSHip.hasDied())
            {
                print(this.name + " healed " + PossibleTargets[t]);
                tSHip.heal(this.attackdamage);
            }
        }
    }
}
