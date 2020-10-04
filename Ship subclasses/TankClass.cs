using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankClass : Ship
{
    // Start is called before the first frame update
    void Start()
    {
        this.health = 9;
        this.attackdamage = 3;
        this.speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
