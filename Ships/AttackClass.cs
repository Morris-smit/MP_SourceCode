using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackClass : Ship
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void InitializeStats()
    {
        this.health = 5;
        this.attackdamage = 7;
        this.speed = 6;
    }

}
