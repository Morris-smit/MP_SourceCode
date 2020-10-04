using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackClass : Ship
{
    // Start is called before the first frame update
    void Start()
    {
        this.health = 5;
        this.attackdamage = 9;
        this.speed = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
